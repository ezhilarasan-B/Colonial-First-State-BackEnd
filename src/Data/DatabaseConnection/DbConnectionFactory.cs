using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using UserDirectory.Api.Constants;

namespace UserDirectory.Api.Data.DatabaseConnection;

/// <summary>
/// ADO.NET SQLite / Turso Connection Factory implementation.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(DatabaseConstants.DefaultConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Database connection string '{DatabaseConstants.DefaultConnectionStringName}' was not configured.");
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    /// <inheritdoc />
    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
