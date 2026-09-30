using UserDirectory.Api.Contracts.Auth.Models;

namespace UserDirectory.Api.Contracts.Interfaces.Repositories;

/// <summary>
/// Data access abstraction for Authentication and User credentials lookup.
/// </summary>
public interface IAuthRepository
{
    Task<AuthUserModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<AuthUserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
