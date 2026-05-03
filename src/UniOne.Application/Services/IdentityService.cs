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

    public IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
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
}
