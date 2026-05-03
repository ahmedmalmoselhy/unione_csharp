namespace UniOne.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, UserDto User);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string Password);

public record UserTokenDto(
    long Id,
    string Name,
    DateTime CreatedAt,
    DateTime? LastUsedAt,
    bool IsCurrent
);

public record UserDto(
...
    long Id,
    string Email,
    string FirstName,
    string LastName,
    string? NationalId,
    string? Phone,
    IEnumerable<string> Roles
);
