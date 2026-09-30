using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Data.DatabaseConnection;

namespace UserDirectory.Api.Repositories;

/// <summary>
/// ADO.NET SQLite implementation of IAuthUserRepository.
/// Handles CRUD operations for the AuthUsers table.
/// Separate from AuthRepository which is used only for authentication/login.
/// </summary>
public class AuthUserRepository : IAuthUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AuthUserRepository> _logger;

    public AuthUserRepository(IDbConnectionFactory connectionFactory, ILogger<AuthUserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AuthUserModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string query = @"
            SELECT id, username, passwordHash, passwordSalt, email, role,
                   createdDate, createdBy,
                   modifiedDate, modifiedBy, modifiedOn,
                   deletedDate, deletedBy, deletedOn
            FROM AuthUsers
            WHERE deletedDate IS NULL
              AND deletedBy IS NULL
            ORDER BY createdDate DESC;";

        await using var command = new SqliteCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<AuthUserModel>();
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapReaderToAuthUser(reader));
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<AuthUserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string query = @"
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
        return await reader.ReadAsync(cancellationToken) ? MapReaderToAuthUser(reader) : null;
    }

    /// <inheritdoc />
    public async Task<AuthUserModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string query = @"
            SELECT id, username, passwordHash, passwordSalt, email, role,
                   createdDate, createdBy,
                   modifiedDate, modifiedBy, modifiedOn,
                   deletedDate, deletedBy, deletedOn
            FROM AuthUsers
            WHERE username = @username
              AND deletedDate IS NULL
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Username, username));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapReaderToAuthUser(reader) : null;
    }

    /// <inheritdoc />
    public async Task<AuthUserModel> CreateAsync(AuthUserModel authUser, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string insertQuery = @"
            INSERT INTO AuthUsers (username, passwordHash, passwordSalt, email, role, createdDate, createdBy)
            VALUES (@username, @passwordHash, @passwordSalt, @email, @role, @createdDate, @createdBy);
            SELECT last_insert_rowid();";

        await using var command = new SqliteCommand(insertQuery, connection);
        command.Parameters.Add(new SqliteParameter("@username", authUser.Username));
        command.Parameters.Add(new SqliteParameter("@passwordHash", authUser.PasswordHash));
        command.Parameters.Add(new SqliteParameter("@passwordSalt", authUser.PasswordSalt));
        command.Parameters.Add(new SqliteParameter("@email", authUser.Email));
        command.Parameters.Add(new SqliteParameter("@role", authUser.Role));
        command.Parameters.Add(new SqliteParameter("@createdDate", authUser.CreatedDate.ToString("O")));
        command.Parameters.Add(new SqliteParameter("@createdBy", authUser.CreatedBy));

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        authUser.Id = newId;

        _logger.LogInformation("Created new AuthUser with ID {Id}, Username: {Username}", newId, authUser.Username);
        return authUser;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string query = @"
            UPDATE AuthUsers
            SET deletedDate = @deletedDate,
                deletedBy = @deletedBy,
                deletedOn = @deletedOn
            WHERE id = @id
              AND deletedDate IS NULL
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Id, id));
        command.Parameters.Add(new SqliteParameter("@deletedDate", DateTime.UtcNow.ToString("O")));
        command.Parameters.Add(new SqliteParameter("@deletedBy", deletedBy));
        command.Parameters.Add(new SqliteParameter("@deletedOn", DateTime.UtcNow.ToString("O")));

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    private static AuthUserModel MapReaderToAuthUser(SqliteDataReader reader)
    {
        var createdDateStr = reader.GetString(reader.GetOrdinal("createdDate"));
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
            CreatedBy = reader.GetString(reader.GetOrdinal("createdBy")),
        };
    }
}
