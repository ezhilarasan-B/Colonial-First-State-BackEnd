using UserDirectory.Api.Contracts.AuthUser.Requests;
using UserDirectory.Api.Contracts.AuthUser.Responses;

namespace UserDirectory.Api.Contracts.Interfaces.Services;

/// <summary>
/// Service abstraction for AuthUser management operations.
/// </summary>
public interface IAuthUserService
{
    Task<IEnumerable<AuthUserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthUserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthUserResponse> CreateAsync(CreateAuthUserRequest request, string createdBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
