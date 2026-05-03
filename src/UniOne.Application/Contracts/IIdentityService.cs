using Microsoft.AspNetCore.Identity;
using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IIdentityService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync(long userId);
    Task<bool> ForgotPasswordAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request);
    Task<IdentityResult> ChangePasswordAsync(long userId, ChangePasswordRequest request);
    Task<UserDto?> UpdateProfileAsync(long userId, UpdateProfileRequest request);
    Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId);
    Task<bool> RevokeTokenAsync(long userId, long tokenId);
    Task RevokeAllTokensAsync(long userId);
}
