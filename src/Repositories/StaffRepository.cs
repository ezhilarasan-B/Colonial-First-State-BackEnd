using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Models;
using UserDirectory.Api.Data.DatabaseConnection;
using UserDirectory.Api.Contracts.Interfaces.Repositories;

namespace UserDirectory.Api.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(IDbConnectionFactory connectionFactory, ILogger<StaffRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<PagedResult<StaffModel>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        int offset = (pageNumber - 1) * pageSize;

        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        // 1. Get Total Count
        const string countSql = "SELECT COUNT(*) FROM Staff WHERE deletedDate IS NULL AND deletedBy IS NULL;";
        await using var countCmd = new SqliteCommand(countSql, connection);
        var totalCountObj = await countCmd.ExecuteScalarAsync(cancellationToken);
        int totalCount = Convert.ToInt32(totalCountObj);

        // 2. Get Paged Items
        const string querySql = @"
            SELECT id, name, age, city, state, pincode, 
                   createdDate, createdBy, modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM Staff
            WHERE deletedDate IS NULL AND deletedBy IS NULL
            ORDER BY id ASC
            LIMIT @PageSize OFFSET @Offset;";

        await using var queryCmd = new SqliteCommand(querySql, connection);
        queryCmd.Parameters.AddWithValue("@PageSize", pageSize);
        queryCmd.Parameters.AddWithValue("@Offset", offset);

        var list = new List<StaffModel>();
        await using var reader = await queryCmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapReaderToStaff(reader));
        }

        return new PagedResult<StaffModel>(list, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<StaffModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            SELECT id, name, age, city, state, pincode, 
                   createdDate, createdBy, modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM Staff
            WHERE deletedDate IS NULL AND deletedBy IS NULL
            ORDER BY id ASC;";

        await using var cmd = new SqliteCommand(sql, connection);
        var list = new List<StaffModel>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapReaderToStaff(reader));
        }
        return list;
    }

    public async Task<StaffModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            SELECT id, name, age, city, state, pincode, 
                   createdDate, createdBy, modifiedDate, modifiedBy, modifiedOn, 
                   deletedDate, deletedBy, deletedOn
            FROM Staff
            WHERE id = @Id AND deletedDate IS NULL AND deletedBy IS NULL;";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapReaderToStaff(reader);
        }
        return null;
    }

    public async Task<StaffModel> CreateAsync(StaffModel staff, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            INSERT INTO Staff (name, age, city, state, pincode, createdDate, createdBy)
            VALUES (@Name, @Age, @City, @State, @Pincode, @CreatedDate, @CreatedBy);
            SELECT last_insert_rowid();";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Name", staff.Name);
        cmd.Parameters.AddWithValue("@Age", staff.Age);
        cmd.Parameters.AddWithValue("@City", staff.City);
        cmd.Parameters.AddWithValue("@State", staff.State);
        cmd.Parameters.AddWithValue("@Pincode", staff.Pincode);
        cmd.Parameters.AddWithValue("@CreatedDate", staff.CreatedDate);
        cmd.Parameters.AddWithValue("@CreatedBy", staff.CreatedBy);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        staff.Id = Convert.ToInt32(result);
        return staff;
    }

    public async Task<StaffModel?> UpdateAsync(StaffModel staff, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = @"
            UPDATE Staff
            SET name = @Name,
                age = @Age,
                city = @City,
                state = @State,
                pincode = @Pincode,
                modifiedDate = @ModifiedDate,
                modifiedBy = @ModifiedBy,
                modifiedOn = @ModifiedOn
            WHERE id = @Id AND deletedDate IS NULL AND deletedBy IS NULL;";

        await using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", staff.Id);
        cmd.Parameters.AddWithValue("@Name", staff.Name);
        cmd.Parameters.AddWithValue("@Age", staff.Age);
        cmd.Parameters.AddWithValue("@City", staff.City);
        cmd.Parameters.AddWithValue("@State", staff.State);
        cmd.Parameters.AddWithValue("@Pincode", staff.Pincode);
        cmd.Parameters.AddWithValue("@ModifiedDate", staff.ModifiedDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ModifiedBy", staff.ModifiedBy ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ModifiedOn", staff.ModifiedOn ?? (object)DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0 ? staff : null;
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var now = DateTime.UtcNow.ToString("o");
        const string sql = @"
            UPDATE Staff
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

    private static StaffModel MapReaderToStaff(SqliteDataReader reader)
    {
        return new StaffModel
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Age = reader.GetInt32(reader.GetOrdinal("age")),
            City = reader.GetString(reader.GetOrdinal("city")),
            State = reader.GetString(reader.GetOrdinal("state")),
            Pincode = reader.GetString(reader.GetOrdinal("pincode")),
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
