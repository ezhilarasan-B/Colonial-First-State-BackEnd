using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Data.DatabaseConnection;
using UserDirectory.Api.Contracts.Interfaces.Repositories;

namespace UserDirectory.Api.Repositories;

/// <summary>
/// ADO.NET SQLite implementation of IAuthRepository.
/// Enforces soft-delete filtering on authentication credentials.
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(IDbConnectionFactory connectionFactory, ILogger<AuthRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthUserModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            SELECT id, username, passwordHash, passwordSalt, email, role, 
                   createdDate, createdBy, 
                   modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM AuthUsers
            WHERE username COLLATE NOCASE = @username 
              AND deletedDate IS NULL 
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Username, username));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapReaderToAuthUser(reader);
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<AuthUserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            SELECT id, username, passwordHash, passwordSalt, email, role, 
                   createdDate, createdBy, 
                   modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM AuthUsers
            WHERE id = @id 
              AND deletedDate IS NULL 
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Id, id));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapReaderToAuthUser(reader);
        }

        return null;
    }

    private static AuthUserModel MapReaderToAuthUser(SqliteDataReader reader)
    {
        var createdDateOrdinal = reader.GetOrdinal("createdDate");
        var createdDateStr = reader.GetString(createdDateOrdinal);
        DateTime.TryParse(createdDateStr, out var createdDate);

        return new AuthUserModel
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            PasswordHash = reader.GetString(reader.GetOrdinal("passwordHash")),
            PasswordSalt = reader.GetString(reader.GetOrdinal("passwordSalt")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Role = reader.GetString(reader.GetOrdinal("role")),
            CreatedDate = createdDate,
            CreatedBy = reader.GetString(reader.GetOrdinal("createdBy"))
        };
    }
}
