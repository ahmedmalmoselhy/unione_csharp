namespace UniOne.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(long userId, string email, IEnumerable<string> roles);
}
