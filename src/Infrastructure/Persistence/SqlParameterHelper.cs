using Microsoft.Data.Sqlite;

namespace UserDirectory.Api.Infrastructure.Persistence;

public static class SqlParameterHelper
{
    public static void AddNullableParameter(SqliteCommand command, string parameterName, object? value)
    {
        command.Parameters.AddWithValue(parameterName, value ?? DBNull.Value);
    }
}
