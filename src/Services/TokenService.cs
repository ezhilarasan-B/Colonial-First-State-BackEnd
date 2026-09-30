using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Contracts.Auth.Responses;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Infrastructure.Authentication;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Services;

/// <summary>
/// Service managing JWT creation and stateless refresh token rotation using a single secret key.
/// Refresh tokens are NOT stored in the backend database.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthRepository _authRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IJwtTokenService jwtTokenService,
        IAuthRepository authRepository,
        IConfiguration configuration,
        ILogger<TokenService> logger)
    {
        _jwtTokenService = jwtTokenService;
        _authRepository = authRepository;
        _logger = logger;

        _jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(_jwtOptions);
    }

    public Task<RefreshTokenResponse> GenerateTokensAsync(AuthUserModel user, CancellationToken cancellationToken = default)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(user);

        return Task.FromResult(new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtOptions.AccessTokenExpirationMinutes * 60
        });
    }

    public async Task<RefreshTokenResponse> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedException("Refresh token is required.");
        }

        // Validate the JWT refresh token using the single secret key
        var principal = _jwtTokenService.ValidateToken(refreshToken, validateLifetime: true);
        if (principal == null)
        {
            _logger.LogWarning("Refresh token validation failed: invalid signature or expired.");
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        // Verify token_type claim
        var tokenType = principal.FindFirst("token_type")?.Value;
        if (tokenType != "refresh")
        {
            _logger.LogWarning("Token presented for refresh does not have token_type=refresh claim.");
            throw new UnauthorizedException("Provided token is not a refresh token.");
        }

        // Extract user id
        var userIdStr = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedException("Invalid token identity.");
        }

        // Verify user exists and is active in database
        var user = await _authRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} from refresh token no longer exists.", userId);
            throw new UnauthorizedException("User account not found or has been disabled.");
        }

        // Extract or preserve token family for token rotation tracking
        var tokenFamily = principal.FindFirst("token_family")?.Value ?? Guid.NewGuid().ToString();

        // Issue new rotated Access Token and new rotated Refresh Token
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user, tokenFamily);

        _logger.LogInformation("Successfully rotated JWT refresh token for user {UserId}.", userId);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtOptions.AccessTokenExpirationMinutes * 60
        };
    }
}
