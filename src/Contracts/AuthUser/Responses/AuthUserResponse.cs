namespace UserDirectory.Api.Contracts.AuthUser.Responses;

/// <summary>
/// Response contract for an authentication user record.
/// </summary>
public class AuthUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
