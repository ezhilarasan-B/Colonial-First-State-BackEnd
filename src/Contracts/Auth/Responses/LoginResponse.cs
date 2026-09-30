namespace UserDirectory.Api.Contracts.Auth.Responses;

/// <summary>
/// Response contract returned upon successful user authentication.
/// </summary>
public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserInfoResponse? User { get; set; }

    public string Username
    {
        get => User?.Username ?? string.Empty;
        set
        {
            User ??= new UserInfoResponse();
            User.Username = value;
        }
    }

    public string Role
    {
        get => User?.Role ?? string.Empty;
        set
        {
            User ??= new UserInfoResponse();
            User.Role = value;
        }
    }
}

public class UserInfoResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
