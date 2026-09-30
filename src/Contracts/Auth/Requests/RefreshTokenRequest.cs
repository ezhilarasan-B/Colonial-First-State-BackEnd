namespace UserDirectory.Api.Contracts.Auth.Requests;

/// <summary>
/// Request contract for refreshing an expired access token using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

