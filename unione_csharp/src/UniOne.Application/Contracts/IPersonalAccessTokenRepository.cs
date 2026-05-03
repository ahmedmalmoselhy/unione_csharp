using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IPersonalAccessTokenRepository
{
    Task StoreTokenAsync(long userId, string name, string accessToken, DateTime expiresAt);
    Task<bool> IsTokenActiveAsync(long userId, string accessToken);
    Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId, string? currentAccessToken);
    Task<bool> RevokeTokenAsync(long userId, long tokenId);
    Task<bool> RevokeTokenAsync(long userId, string accessToken);
    Task RevokeAllTokensAsync(long userId);
}
