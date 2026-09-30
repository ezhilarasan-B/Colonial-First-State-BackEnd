using System.Data;

namespace UserDirectory.Api.Data.DatabaseConnection;

/// <summary>
/// Factory abstraction for creating and opening database connections.
/// Allows repository isolation and testability.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new, unopened database connection.
    /// </summary>
    IDbConnection CreateConnection();

    /// <summary>
    /// Creates and opens a new database connection asynchronously.
    /// </summary>
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}

