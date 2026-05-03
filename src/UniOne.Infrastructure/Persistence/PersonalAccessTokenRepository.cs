using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Infrastructure.Persistence;

namespace UniOne.Infrastructure.Persistence;

public class PersonalAccessTokenRepository : IPersonalAccessTokenRepository
{
    private readonly UniOneDbContext _dbContext;

    public PersonalAccessTokenRepository(UniOneDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId)
    {
        var tokens = await _dbContext.PersonalAccessTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tokens.Select(t => new UserTokenDto(
            t.Id,
            t.Name,
            t.CreatedAt,
            t.LastUsedAt,
            false
        ));
    }

    public async Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        var token = await _dbContext.PersonalAccessTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId && t.UserId == userId);

        if (token == null) return false;

        _dbContext.PersonalAccessTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task RevokeAllTokensAsync(long userId)
    {
        var tokens = await _dbContext.PersonalAccessTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();

        _dbContext.PersonalAccessTokens.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync();
    }
}
