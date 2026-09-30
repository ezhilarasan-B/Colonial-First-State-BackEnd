using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Client.Models;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Data.DatabaseConnection;
using UserDirectory.Api.Contracts.Interfaces.Repositories;

namespace UserDirectory.Api.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ClientRepository> _logger;

    public ClientRepository(IDbConnectionFactory connectionFactory, ILogger<ClientRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<PagedResult<ClientModel>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        int offset = (pageNumber - 1) * pageSize;

        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string countSql = "SELECT COUNT(*) FROM Clients WHERE deletedDate IS NULL AND deletedBy IS NULL;";
        await using var countCmd = new SqliteCommand(countSql, connection);
        var totalCountObj = await countCmd.ExecuteScalarAsync(cancellationToken);
        int totalCount = Convert.ToInt32(totalCountObj);

        const string querySql = @"
            SELECT c.id, c.name, c.email, c.phone, c.company, c.staffId,
                   COALESCE(s.name, '') AS staffName,
                   c.createdDate, c.createdBy, c.modifiedDate, c.modifiedBy, c.modifiedOn,
                   c.deletedDate, c.deletedBy, c.deletedOn
            FROM Clients c
            LEFT JOIN Staff s ON c.staffId = s.id
            WHERE c.deletedDate IS NULL AND c.deletedBy IS NULL
            ORDER BY c.id ASC
            LIMIT @PageSize OFFSET @Offset;";

        await using var queryCmd = new SqliteCommand(querySql, connection);
        queryCmd.Parameters.AddWithValue("@PageSize", pageSize);
        queryCmd.Parameters.AddWithValue("@Offset", offset);

        var list = new List<ClientModel>();
        await using var reader = await queryCmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapReaderToClient(reader));
        }

        return new PagedResult<ClientModel>(list, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<ClientModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            SELECT c.id, c.name, c.email, c.phone, c.company, c.staffId,
                   COALESCE(s.name, '') AS staffName,
                   c.createdDate, c.createdBy, c.modifiedDate, c.modifiedBy, c.modifiedOn,
                   c.deletedDate, c.deletedBy, c.deletedOn
            FROM Clients c
            LEFT JOIN Staff s ON c.staffId = s.id
            WHERE c.deletedDate IS NULL AND c.deletedBy IS NULL
            ORDER BY c.id ASC;";

        await using var cmd = new SqliteCommand(sql, connection);
        var list = new List<ClientModel>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapReaderToClient(reader));
        }
        return list;
    }

    public async Task<ClientModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            SELECT c.id, c.name, c.email, c.phone, c.company, c.staffId,
                   COALESCE(s.name, '') AS staffName,
                   c.createdDate, c.createdBy, c.modifiedDate, c.modifiedBy, c.modifiedOn,
                   c.deletedDate, c.deletedBy, c.deletedOn
            FROM Clients c
            LEFT JOIN Staff s ON c.staffId = s.id
            WHERE c.id = @Id AND c.deletedDate IS NULL AND c.deletedBy IS NULL;";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapReaderToClient(reader);
        }
        return null;
    }

    public async Task<ClientModel> CreateAsync(ClientModel client, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            INSERT INTO Clients (name, email, phone, company, staffId, createdDate, createdBy)
            VALUES (@Name, @Email, @Phone, @Company, @StaffId, @CreatedDate, @CreatedBy);
            SELECT last_insert_rowid();";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Name", client.Name);
        cmd.Parameters.AddWithValue("@Email", client.Email);
        cmd.Parameters.AddWithValue("@Phone", client.Phone);
        cmd.Parameters.AddWithValue("@Company", client.Company);
        cmd.Parameters.AddWithValue("@StaffId", client.StaffId);
        cmd.Parameters.AddWithValue("@CreatedDate", client.CreatedDate);
        cmd.Parameters.AddWithValue("@CreatedBy", client.CreatedBy);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        client.Id = Convert.ToInt32(result);

        // Fetch staff name for completeness
        const string staffNameSql = "SELECT name FROM Staff WHERE id = @StaffId;";
        await using var staffCmd = new SqliteCommand(staffNameSql, connection);
        staffCmd.Parameters.AddWithValue("@StaffId", client.StaffId);
        var staffNameObj = await staffCmd.ExecuteScalarAsync(cancellationToken);
        client.StaffName = staffNameObj?.ToString() ?? string.Empty;

        return client;
    }

    public async Task<ClientModel?> UpdateAsync(ClientModel client, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            UPDATE Clients
            SET name = @Name,
                email = @Email,
                phone = @Phone,
                company = @Company,
                staffId = @StaffId,
                modifiedDate = @ModifiedDate,
                modifiedBy = @ModifiedBy,
                modifiedOn = @ModifiedOn
            WHERE id = @Id AND deletedDate IS NULL AND deletedBy IS NULL;";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", client.Id);
        cmd.Parameters.AddWithValue("@Name", client.Name);
        cmd.Parameters.AddWithValue("@Email", client.Email);
        cmd.Parameters.AddWithValue("@Phone", client.Phone);
        cmd.Parameters.AddWithValue("@Company", client.Company);
        cmd.Parameters.AddWithValue("@StaffId", client.StaffId);
        cmd.Parameters.AddWithValue("@ModifiedDate", client.ModifiedDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ModifiedBy", client.ModifiedBy ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ModifiedOn", client.ModifiedOn ?? (object)DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected > 0)
        {
            const string staffNameSql = "SELECT name FROM Staff WHERE id = @StaffId;";
            await using var staffCmd = new SqliteCommand(staffNameSql, connection);
            staffCmd.Parameters.AddWithValue("@StaffId", client.StaffId);
            var staffNameObj = await staffCmd.ExecuteScalarAsync(cancellationToken);
            client.StaffName = staffNameObj?.ToString() ?? string.Empty;
            return client;
        }

        return null;
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var now = DateTime.UtcNow.ToString("o");
        const string sql = @"
            UPDATE Clients
            SET deletedDate = @DeletedDate,
                deletedBy = @DeletedBy,
                deletedOn = @DeletedOn
            WHERE id = @Id AND deletedDate IS NULL AND deletedBy IS NULL;";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@DeletedDate", now);
        cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);
        cmd.Parameters.AddWithValue("@DeletedOn", now);

        var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> HasClientsAssignedToStaffAsync(int staffId, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        const string sql = "SELECT COUNT(*) FROM Clients WHERE staffId = @StaffId AND deletedDate IS NULL AND deletedBy IS NULL;";
        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@StaffId", staffId);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    public async Task<List<int>> GetAssignedStaffIdsAsync(IEnumerable<int> staffIds, CancellationToken cancellationToken = default)
    {
        var idList = staffIds.Distinct().ToList();
        if (!idList.Any()) return new List<int>();

        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var inClause = string.Join(",", idList);
        var sql = $"SELECT DISTINCT staffId FROM Clients WHERE staffId IN ({inClause}) AND deletedDate IS NULL AND deletedBy IS NULL;";
        await using var cmd = new SqliteCommand(sql, connection);
        var list = new List<int>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(reader.GetInt32(0));
        }
        return list;
    }

    private static ClientModel MapReaderToClient(SqliteDataReader reader)
    {
        return new ClientModel
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Phone = reader.GetString(reader.GetOrdinal("phone")),
            Company = reader.GetString(reader.GetOrdinal("company")),
            StaffId = reader.GetInt32(reader.GetOrdinal("staffId")),
            StaffName = reader.GetString(reader.GetOrdinal("staffName")),
            CreatedDate = reader.GetString(reader.GetOrdinal("createdDate")),
            CreatedBy = reader.GetString(reader.GetOrdinal("createdBy")),
            ModifiedDate = reader.IsDBNull(reader.GetOrdinal("modifiedDate")) ? null : reader.GetString(reader.GetOrdinal("modifiedDate")),
            ModifiedBy = reader.IsDBNull(reader.GetOrdinal("modifiedBy")) ? null : reader.GetString(reader.GetOrdinal("modifiedBy")),
            ModifiedOn = reader.IsDBNull(reader.GetOrdinal("modifiedOn")) ? null : reader.GetString(reader.GetOrdinal("modifiedOn")),
            DeletedDate = reader.IsDBNull(reader.GetOrdinal("deletedDate")) ? null : reader.GetString(reader.GetOrdinal("deletedDate")),
            DeletedBy = reader.IsDBNull(reader.GetOrdinal("deletedBy")) ? null : reader.GetString(reader.GetOrdinal("deletedBy")),
            DeletedOn = reader.IsDBNull(reader.GetOrdinal("deletedOn")) ? null : reader.GetString(reader.GetOrdinal("deletedOn"))
        };
    }
}
