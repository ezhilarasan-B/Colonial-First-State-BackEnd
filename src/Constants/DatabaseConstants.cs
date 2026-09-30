namespace UserDirectory.Api.Constants;

/// <summary>
/// Centralized database configuration keys, parameter names, and table names.
/// </summary>
public static class DatabaseConstants
{
    public const string DefaultConnectionStringName = "DefaultConnection";

    public static class Tables
    {
        public const string Staff = "Staff";
        public const string Users = "Staff"; // Alias pointing to Staff
        public const string AuthUsers = "AuthUsers";
        public const string RefreshTokens = "RefreshTokens";
    }

    public static class ParameterNames
    {
        public const string Id = "@id";
        public const string Name = "@name";
        public const string Age = "@age";
        public const string City = "@city";
        public const string State = "@state";
        public const string Pincode = "@pincode";

        // Audit & Lifecycle parameters
        public const string CreatedDate = "@createdDate";
        public const string CreatedBy = "@createdBy";
        public const string ModifiedDate = "@modifiedDate";
        public const string ModifiedBy = "@modifiedBy";
        public const string ModifiedOn = "@modifiedOn";
        public const string DeletedDate = "@deletedDate";
        public const string DeletedBy = "@deletedBy";
        public const string DeletedOn = "@deletedOn";

        // Auth parameters
        public const string Username = "@username";
        public const string PasswordHash = "@passwordHash";
        public const string PasswordSalt = "@passwordSalt";
        public const string Email = "@email";
        public const string Role = "@role";

        // Refresh Token parameters
        public const string UserId = "@userId";
        public const string TokenHash = "@tokenHash";
        public const string ExpiresAt = "@expiresAt";
        public const string IsRevoked = "@isRevoked";
        public const string RevokedAt = "@revokedAt";
        public const string ReplacedByToken = "@replacedByToken";
    }
}
