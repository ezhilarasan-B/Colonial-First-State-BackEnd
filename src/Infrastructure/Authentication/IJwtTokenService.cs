using System.Security.Claims;
using UserDirectory.Api.Contracts.Auth.Models;

namespace UserDirectory.Api.Infrastructure.Authentication;

public interface IJwtTokenService
{
    string GenerateAccessToken(AuthUserModel user);
    string GenerateRefreshToken(AuthUserModel user, string? tokenFamily = null);
    ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
    string HashPassword(string password, string salt);
    string GenerateSalt();
    bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt);
}
