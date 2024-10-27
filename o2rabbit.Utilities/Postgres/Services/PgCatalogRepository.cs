using System.Runtime.CompilerServices;
using System.Text;
using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;
using o2rabbit.Utilities.Postgres.Models;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgCatalogRepository : IPgCatalogRepository
{
    public async Task<List<QualifiedTableName>> GetAllTableNamesAsync(string connectionString,
        string? schemaName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        //Throws ArgumentException, if connectionstring is invalid.
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);

        await using var command =
            GetNpgsqlCommand(schemaName, connection);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var ret = new List<QualifiedTableName>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            ret.Add(new QualifiedTableName(reader.GetString(0), reader.GetString(1)));
        }

        return ret;
    }

    private NpgsqlCommand GetNpgsqlCommand(string? schemaName, NpgsqlConnection connection)
    {
        NpgsqlCommand? command = null;
        var sb = new StringBuilder()
            .Append(
                @$"
SELECT schemaname, tablename FROM pg_catalog.pg_tables 
WHERE schemaname NOT IN ('information_schema', 'pg_catalog')"
            );

        command = new NpgsqlCommand();

        if (schemaName != null)
        {
            sb.Append("AND schemaname = $1");
            command.Parameters.Add(new() { Value = schemaName, });
        }

        command.CommandText = sb.ToString();
        command.Connection = connection;

        return command;
    }
}