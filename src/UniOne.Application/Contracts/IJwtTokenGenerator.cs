using UniOne.Domain.Entities;

namespace UniOne.Application.Contracts;

public interface IJwtTokenGenerator
{
    GeneratedJwtToken GenerateToken(User user, IEnumerable<string> roles);
}

public record GeneratedJwtToken(string AccessToken, DateTime ExpiresAt);
