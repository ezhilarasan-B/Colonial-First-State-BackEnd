using MediatR;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Auth.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Auth.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand delegating token rotation to IAuthService.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IAuthService _authService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(IAuthService authService, ILogger<RefreshTokenCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling RefreshTokenCommand");
        return await _authService.RefreshTokenAsync(command.Request, cancellationToken);
    }
}

