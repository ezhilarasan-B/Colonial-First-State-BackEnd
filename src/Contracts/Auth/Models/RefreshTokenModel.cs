namespace UserDirectory.Api.Contracts.Auth.Models;

/// <summary>
/// Domain model representing a refresh token entity with audit and soft-delete properties.
/// </summary>
public class RefreshTokenModel
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

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
    public bool IsActive => !IsRevoked && RevokedAt == null && !IsExpired && !IsDeleted;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public DateTime CreatedAt => CreatedDate;
}
