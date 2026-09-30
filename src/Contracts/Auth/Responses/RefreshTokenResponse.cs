namespace UserDirectory.Api.Contracts.Auth.Responses;

/// <summary>
/// Response contract returned upon successful refresh token rotation.
/// </summary>
public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

