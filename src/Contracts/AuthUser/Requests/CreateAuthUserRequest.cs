namespace UserDirectory.Api.Contracts.AuthUser.Requests;

/// <summary>
/// Request contract for creating a new authentication user.
/// </summary>
public class CreateAuthUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
