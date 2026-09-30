using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Contracts.AuthUser.Requests;

namespace UserDirectory.Api.Contracts.Interfaces.Repositories;

/// <summary>
/// Data access abstraction for AuthUser management (CRUD operations on the AuthUsers table).
/// Separate from IAuthRepository which is used for authentication/login only.
/// </summary>
public interface IAuthUserRepository
{
    Task<IEnumerable<AuthUserModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthUserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthUserModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<AuthUserModel> CreateAsync(AuthUserModel authUser, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
