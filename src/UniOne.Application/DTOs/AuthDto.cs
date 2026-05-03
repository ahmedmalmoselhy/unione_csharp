namespace UniOne.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, UserDto User);

public record UserDto(
    long Id,
    string Email,
    string FirstName,
    string LastName,
    string? NationalId,
    string? Phone,
    IEnumerable<string> Roles
);
