using System.Runtime.CompilerServices;
using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgCatalogRepository : IPgCatalogRepository
{
    public async Task<IEnumerable<string>> GetTableNamesAsync(string connectionString, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        //Throws ArgumentException, if connectionstring is invalid.
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        
        var commandText =
            @$"
SELECT tablename FROM pg_catalog.pg_tables 
WHERE schemaname NOT IN ('information_schema', 'pg_catalog')
{(schemaName is null ? string.Empty: $"AND schemaname = {schemaName}")}";
        
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await using var command = new NpgsqlCommand(commandText, connection);
        
        await connection.OpenAsync().ConfigureAwait(false);
        
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        var ret = new List<string>();
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
           ret.Add(reader.GetString(0));
        }
        return ret;
    }
}