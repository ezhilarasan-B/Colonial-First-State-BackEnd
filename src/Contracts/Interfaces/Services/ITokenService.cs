using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Contracts.Auth.Responses;

namespace UserDirectory.Api.Contracts.Interfaces.Services;

public interface ITokenService
{
    Task<RefreshTokenResponse> GenerateTokensAsync(AuthUserModel user, CancellationToken cancellationToken = default);
    Task<RefreshTokenResponse> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
