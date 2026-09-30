using UserDirectory.Api.Contracts.Auth.Models;

namespace UserDirectory.Api.Contracts.Interfaces.Repositories;

/// <summary>
/// Data access abstraction for Refresh Token persistence and rotation operations.
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task CreateAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string tokenHash, DateTime revokedAt, string? replacedByToken = null, CancellationToken cancellationToken = default);
}

