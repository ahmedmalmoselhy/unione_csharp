using Microsoft.AspNetCore.Identity;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPersonalAccessTokenRepository _tokenRepository;

    public IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IPersonalAccessTokenRepository tokenRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _tokenRepository = tokenRepository;
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
        return await _tokenRepository.GetActiveTokensAsync(userId);
    }

    public async Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        return await _tokenRepository.RevokeTokenAsync(userId, tokenId);
    }

    public async Task RevokeAllTokensAsync(long userId)
    {
        await _tokenRepository.RevokeAllTokensAsync(userId);
    }
}
