namespace UserDirectory.Api.Constants;

/// <summary>
/// Centralized Authentication and JWT constants.
/// </summary>
public static class AuthConstants
{
    public const string BearerScheme = "Bearer";
    public const string AuthorizationHeader = "Authorization";
    public const string BearerPrefix = "Bearer ";

    public const string JwtConfigSection = "Jwt";
    public const string CorsConfigSection = "Cors";

    public const string DefaultRole = "User";
    public const string AdminRole = "Admin";

    public const int DefaultAccessTokenExpirationMinutes = 15;
    public const int DefaultRefreshTokenExpirationDays = 7;
    public const int RefreshTokenByteLength = 64;
}

