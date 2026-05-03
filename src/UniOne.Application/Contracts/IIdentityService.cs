using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IIdentityService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync(long userId);
}
