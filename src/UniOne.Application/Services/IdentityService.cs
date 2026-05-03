using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;
using UniOne.Infrastructure.Persistence;

namespace UniOne.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UniOneDbContext _dbContext;

    public IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        UniOneDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dbContext = dbContext;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);

        return new AuthResponse(token, new UserDto(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.NationalId,
            user.Phone,
            roles
        ));
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<UserDto?> GetCurrentUserAsync(long userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.NationalId,
            user.Phone,
            roles
        );
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send email with token. For now, we just return true.
        // In a real app, we would use an IEmailService.

        return true;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        return await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
    }

    public async Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId)
    {
        var tokens = await _dbContext.UserTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tokens.Select(t => new UserTokenDto(
            t.Id,
            t.Name,
            t.CreatedAt,
            t.LastUsedAt,
            false // TODO: Determine if it's current
        ));
    }

    public async Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        var token = await _dbContext.UserTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId && t.UserId == userId);

        if (token == null) return false;

        _dbContext.UserTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task RevokeAllTokensAsync(long userId)
    {
        var tokens = await _dbContext.UserTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();

        _dbContext.UserTokens.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync();
    }
}
