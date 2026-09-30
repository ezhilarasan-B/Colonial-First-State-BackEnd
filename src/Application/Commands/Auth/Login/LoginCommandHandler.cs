using MediatR;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Auth.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Auth.Login;

/// <summary>
/// Handler for LoginCommand delegating authentication business logic to IAuthService.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IAuthService authService, ILogger<LoginCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling LoginCommand for username: {Username}", command.Request.Username);
        return await _authService.LoginAsync(command.Request, cancellationToken);
    }
}

