namespace UserDirectory.Api.Contracts.Auth.Requests;

/// <summary>
/// Request contract for authenticating with username and password.
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

