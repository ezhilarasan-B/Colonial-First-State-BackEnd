using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Data.DatabaseConnection;
using UserDirectory.Api.Contracts.Interfaces.Repositories;

namespace UserDirectory.Api.Repositories;

/// <summary>
/// ADO.NET SQLite implementation of IRefreshTokenRepository.
/// Enforces soft-delete filtering on token operations.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory, ILogger<RefreshTokenRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RefreshTokenModel?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            SELECT id, userId, tokenHash, expiresAt, isRevoked, revokedAt, replacedByToken,
                   createdDate, createdBy, 
                   modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM RefreshTokens
            WHERE tokenHash = @tokenHash 
              AND deletedDate IS NULL 
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.TokenHash, tokenHash));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var expiresOrdinal = reader.GetOrdinal("expiresAt");
            var revokedOrdinal = reader.GetOrdinal("revokedAt");
            var replacedOrdinal = reader.GetOrdinal("replacedByToken");
            var createdDateOrdinal = reader.GetOrdinal("createdDate");
            var isRevokedOrdinal = reader.GetOrdinal("isRevoked");

            var expiresStr = reader.GetString(expiresOrdinal);
            DateTime.TryParse(expiresStr, out var expiresAt);

            DateTime? revokedAt = null;
            if (!reader.IsDBNull(revokedOrdinal))
            {
                if (DateTime.TryParse(reader.GetString(revokedOrdinal), out var parsedRevoked))
                {
                    revokedAt = parsedRevoked;
                }
            }

            var createdDateStr = reader.GetString(createdDateOrdinal);
            DateTime.TryParse(createdDateStr, out var createdDate);

            return new RefreshTokenModel
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                UserId = reader.GetString(reader.GetOrdinal("userId")),
                TokenHash = reader.GetString(reader.GetOrdinal("tokenHash")),
                ExpiresAt = expiresAt,
                IsRevoked = reader.GetInt64(isRevokedOrdinal) == 1,
                RevokedAt = revokedAt,
                ReplacedByToken = reader.IsDBNull(replacedOrdinal) ? null : reader.GetString(replacedOrdinal),
                CreatedDate = createdDate,
                CreatedBy = reader.GetString(reader.GetOrdinal("createdBy"))
            };
        }

        return null;
    }

    /// <inheritdoc />
    public async Task CreateAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            INSERT INTO RefreshTokens (
                id, userId, tokenHash, expiresAt, isRevoked, revokedAt, replacedByToken,
                createdDate, createdBy, 
                modifiedDate, modifiedBy, modifiedOn, 
                deletedDate, deletedBy, deletedOn
            ) VALUES (
                @id, @userId, @tokenHash, @expiresAt, @isRevoked, @revokedAt, @replacedByToken,
                @createdDate, @createdBy, 
                @modifiedDate, @modifiedBy, @modifiedOn, 
                @deletedDate, @deletedBy, @deletedOn
            );";

        await using var command = new SqliteCommand(query, connection);
        
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Id, refreshToken.Id));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.UserId, refreshToken.UserId));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.TokenHash, refreshToken.TokenHash));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ExpiresAt, refreshToken.ExpiresAt.ToString("O")));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.IsRevoked, refreshToken.IsRevoked ? 1 : 0));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.RevokedAt, (object?)refreshToken.RevokedAt?.ToString("O") ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ReplacedByToken, (object?)refreshToken.ReplacedByToken ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.CreatedDate, refreshToken.CreatedDate.ToString("O")));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.CreatedBy, refreshToken.CreatedBy));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedDate, (object?)refreshToken.ModifiedDate?.ToString("O") ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedBy, (object?)refreshToken.ModifiedBy ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedOn, (object?)refreshToken.ModifiedOn ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.DeletedDate, (object?)refreshToken.DeletedDate?.ToString("O") ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.DeletedBy, (object?)refreshToken.DeletedBy ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.DeletedOn, (object?)refreshToken.DeletedOn ?? DBNull.Value));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            UPDATE RefreshTokens
            SET tokenHash = @tokenHash,
                expiresAt = @expiresAt,
                isRevoked = @isRevoked,
                revokedAt = @revokedAt,
                replacedByToken = @replacedByToken,
                modifiedDate = @modifiedDate,
                modifiedBy = @modifiedBy,
                modifiedOn = @modifiedOn
            WHERE id = @id AND deletedDate IS NULL AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.Id, refreshToken.Id));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.TokenHash, refreshToken.TokenHash));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ExpiresAt, refreshToken.ExpiresAt.ToString("O")));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.IsRevoked, refreshToken.IsRevoked ? 1 : 0));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.RevokedAt, (object?)refreshToken.RevokedAt?.ToString("O") ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ReplacedByToken, (object?)refreshToken.ReplacedByToken ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedDate, (object?)refreshToken.ModifiedDate?.ToString("O") ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedBy, (object?)refreshToken.ModifiedBy ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedOn, (object?)refreshToken.ModifiedOn ?? DBNull.Value));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string tokenHash, DateTime revokedAt, string? replacedByToken = null, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        
        var query = @"
            UPDATE RefreshTokens
            SET isRevoked = 1,
                revokedAt = @revokedAt,
                replacedByToken = @replacedByToken,
                modifiedDate = @modifiedDate,
                modifiedBy = @modifiedBy,
                modifiedOn = @modifiedOn
            WHERE tokenHash = @tokenHash 
              AND isRevoked = 0 
              AND deletedDate IS NULL 
              AND deletedBy IS NULL;";

        await using var command = new SqliteCommand(query, connection);
        
        var isoTimestamp = revokedAt.ToString("O");
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.TokenHash, tokenHash));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.RevokedAt, isoTimestamp));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ReplacedByToken, (object?)replacedByToken ?? DBNull.Value));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedDate, isoTimestamp));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedBy, "System"));
        command.Parameters.Add(new SqliteParameter(DatabaseConstants.ParameterNames.ModifiedOn, isoTimestamp));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
