using System.Runtime.CompilerServices;
using System.Text;
using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgCatalogRepository : IPgCatalogRepository
{
    public async Task<IEnumerable<string>> GetAllTableNamesAsync(string connectionString, string? schemaName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        //Throws ArgumentException, if connectionstring is invalid.
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);

        await using var command =
            await GetNpgsqlCommandAsync(schemaName, connection, cancellationToken).ConfigureAwait(false);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var ret = new List<string>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            ret.Add(reader.GetString(0));
        }

        return ret;
    }

    private async ValueTask<NpgsqlCommand> GetNpgsqlCommandAsync(string? schemaName, NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        NpgsqlCommand? command = null;
        var sb = new StringBuilder()
            .Append(
                @$"
SELECT tablename FROM pg_catalog.pg_tables 
WHERE schemaname NOT IN ('information_schema', 'pg_catalog')"
            );

        var sqlParam = new NpgsqlParameter("@schema", schemaName);
        command = new NpgsqlCommand();

        if (schemaName != null)
        {
            sb.Append($"AND schemaname = @schema");
            command.Parameters.Add(sqlParam);
        }

        command.CommandText = sb.ToString();
        command.Connection = connection;

        return command;
    }
}