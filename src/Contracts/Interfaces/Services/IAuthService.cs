using UserDirectory.Api.Contracts.Auth.Requests;
using UserDirectory.Api.Contracts.Auth.Responses;

namespace UserDirectory.Api.Contracts.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
