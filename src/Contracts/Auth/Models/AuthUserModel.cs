namespace UserDirectory.Api.Contracts.Auth.Models;

/// <summary>
/// Domain model representing an authenticated user account with integer ID, audit tracking, and soft-delete properties.
/// </summary>
public class AuthUserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    // Audit and Soft Delete Fields
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedOn { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedOn { get; set; }

    public bool IsDeleted => DeletedDate != null || !string.IsNullOrWhiteSpace(DeletedBy);
    public DateTime CreatedAt => CreatedDate;
}
