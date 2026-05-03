using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Infrastructure.Persistence;

public class PersonalAccessTokenRepository : IPersonalAccessTokenRepository
{
    private readonly UniOneDbContext _dbContext;

    public PersonalAccessTokenRepository(UniOneDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task StoreTokenAsync(long userId, string name, string accessToken, DateTime expiresAt)
    {
        _dbContext.PersonalAccessTokens.Add(new PersonalAccessToken
        {
            UserId = userId,
            Name = name,
            TokenHash = HashToken(accessToken),
            ExpiresAt = expiresAt
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsTokenActiveAsync(long userId, string accessToken)
    {
        var tokenHash = HashToken(accessToken);
        var now = DateTime.UtcNow;

        return await _dbContext.PersonalAccessTokens
            .AnyAsync(t => t.UserId == userId &&
                           t.TokenHash == tokenHash &&
                           (t.ExpiresAt == null || t.ExpiresAt > now));
    }

    public async Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId, string? currentAccessToken)
    {
        var currentTokenHash = currentAccessToken == null ? null : HashToken(currentAccessToken);
        var now = DateTime.UtcNow;

        var tokens = await _dbContext.PersonalAccessTokens
            .Where(t => t.UserId == userId && (t.ExpiresAt == null || t.ExpiresAt > now))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tokens.Select(t => new UserTokenDto(
            t.Id,
            t.Name,
            t.CreatedAt,
            t.LastUsedAt,
            currentTokenHash != null && t.TokenHash == currentTokenHash
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

    public async Task<bool> RevokeTokenAsync(long userId, string accessToken)
    {
        var tokenHash = HashToken(accessToken);
        var token = await _dbContext.PersonalAccessTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenHash == tokenHash);

        if (token == null) return false;

        _dbContext.PersonalAccessTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
