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
        if (user == null || !user.IsActive) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);
        await _tokenRepository.StoreTokenAsync(user.Id, "API token", token.AccessToken, token.ExpiresAt);

        return new AuthResponse(token.AccessToken, MapToDto(user, roles));
    }

    public async Task LogoutAsync(long userId, string accessToken)
    {
        await _tokenRepository.RevokeTokenAsync(userId, accessToken);
        await _signInManager.SignOutAsync();
    }

    public async Task<UserDto?> GetCurrentUserAsync(long userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles);
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // TODO: Send email
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

    public async Task<IdentityResult> ChangePasswordAsync(long userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);
        }

        return result;
    }

    public async Task<UserDto?> UpdateProfileAsync(long userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        if (request.Phone != null) user.Phone = request.Phone;
        if (request.DateOfBirth != null) user.DateOfBirth = request.DateOfBirth;
        if (request.AvatarPath != null) user.AvatarPath = request.AvatarPath;

        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles);
    }

    public async Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId, string? currentAccessToken)
    {
        return await _tokenRepository.GetActiveTokensAsync(userId, currentAccessToken);
    }

    public async Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        return await _tokenRepository.RevokeTokenAsync(userId, tokenId);
    }

    public async Task RevokeAllTokensAsync(long userId)
    {
        await _tokenRepository.RevokeAllTokensAsync(userId);
    }

    private UserDto MapToDto(User user, IEnumerable<string> roles)
    {
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
}
