using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;
using o2rabbit.Utilities.Postgres.Models;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgDdlService : IPgDdlService
{
    public NpgsqlCommand GenerateTruncateTableCommand(string schemaName, string tableName,
        NpgsqlConnection? connection = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        var command = new NpgsqlCommand();
        if (connection != null)
            command.Connection = connection;

        command.CommandText = $"TRUNCATE TABLE {schemaName}.{tableName}";

        return command;
    }

    public NpgsqlCommand GenerateTruncateTableCommand(QualifiedTableName tableName, NpgsqlConnection? connection = null)
        => GenerateTruncateTableCommand(tableName.Table, tableName.Schema, connection);

    public string GenerateSqlParameterAlias()
    {
        var guid = Guid.NewGuid().ToString();

        return $"'{guid.ToString()}'";
    }
}