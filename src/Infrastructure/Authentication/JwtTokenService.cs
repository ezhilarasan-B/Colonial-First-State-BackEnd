using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Contracts.Auth.Models;

namespace UserDirectory.Api.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly SymmetricSecurityKey _signingKey;
    private const int Pbkdf2Iterations = 100_000;
    private const int SaltByteSize = 32;
    private const int HashByteSize = 32;

    public JwtTokenService(IConfiguration configuration)
    {
        _jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(_jwtOptions);

        if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured.");
        }

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
    }

    public string GenerateAccessToken(AuthUserModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var role = string.IsNullOrWhiteSpace(user.Role) ? AuthConstants.DefaultRole : user.Role;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, role),
            new("token_type", "access"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        // Granular role-based permissions
        foreach (var permission in GetPermissionsForRole(role))
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(AuthUserModel user, string? tokenFamily = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("token_type", "refresh"),
            new("token_family", tokenFamily ?? Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateIssuer = !string.IsNullOrWhiteSpace(_jwtOptions.Issuer),
            ValidIssuer = _jwtOptions.Issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(_jwtOptions.Audience),
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Pbkdf2Iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(HashByteSize);
        return Convert.ToBase64String(hashBytes);
    }

    public string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltByteSize);
        return Convert.ToBase64String(saltBytes);
    }

    public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        var computedHash = HashPassword(enteredPassword, storedSalt);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(storedHash)
        );
    }

    private static IEnumerable<string> GetPermissionsForRole(string? role)
    {
        return role switch
        {
            "Admin" => new[] { "staff:read", "staff:write", "client:read", "client:write" },
            "StaffReadOnly" => new[] { "staff:read", "client:read" },
            "ReadOnly" => new[] { "staff:read", "client:read" },
            _ => new[] { "staff:read", "client:read" }
        };
    }
}
