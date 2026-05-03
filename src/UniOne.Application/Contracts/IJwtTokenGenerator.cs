using UniOne.Domain.Entities;

namespace UniOne.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IEnumerable<string> roles);
}
