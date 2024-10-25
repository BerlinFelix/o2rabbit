using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgDdlService:IPgDdlService
{
    public NpgsqlCommand GenerateTruncateTableCommand(string schemaName, string tableName, string? connectionString = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        var command = new NpgsqlCommand();
        var parameterName = GenerateSqlParameterAlias();
        command.CommandText = $"TRUNCATE TABLE {parameterName}";
        
        var param = new NpgsqlParameter(parameterName, $"{schemaName}.{tableName}");
        command.Parameters.Add(param);
        
        return command;
    }

    public string GenerateSqlParameterAlias()
    {
        var guid = Guid.NewGuid().ToString();
        
        return $"@{guid}";
    }
}