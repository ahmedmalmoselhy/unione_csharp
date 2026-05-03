using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IPersonalAccessTokenRepository
{
    Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId);
    Task<bool> RevokeTokenAsync(long userId, long tokenId);
    Task RevokeAllTokensAsync(long userId);
}
