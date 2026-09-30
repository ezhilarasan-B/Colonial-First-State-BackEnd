using FluentValidation;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Auth.Requests;
using UserDirectory.Api.Contracts.Auth.Responses;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Infrastructure.Authentication;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Services;
using ValidationException = UserDirectory.Api.Exceptions.ValidationException;

namespace UserDirectory.Api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshValidator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        ITokenService tokenService,
        IJwtTokenService jwtTokenService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RefreshTokenRequest> refreshValidator,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        _jwtTokenService = jwtTokenService;
        _loginValidator = loginValidator;
        _refreshValidator = refreshValidator;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        var user = await _authRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User {Username} not found.", request.Username);
            throw new UnauthorizedException("Invalid username or password.");
        }

        var isPasswordValid = _jwtTokenService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid credentials for user {Username}.", request.Username);
            throw new UnauthorizedException("Invalid username or password.");
        }

        var tokenResponse = await _tokenService.GenerateTokensAsync(user, cancellationToken);

        _logger.LogInformation("User {Username} authenticated successfully.", user.Username);

        return new LoginResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            User = new UserInfoResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _refreshValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }
        return await _tokenService.RotateRefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}
