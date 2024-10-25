using Npgsql;
using o2rabbit.Utilities.Postgres.Abstractions;

namespace o2rabbit.Utilities.Postgres.Services;

public class PgTruncateTableService : IPgTruncateTableService
{
    private readonly string _connectionString;

    public PgTruncateTableService(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        
        _connectionString = connectionString;
    }

    public async Task TruncateTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);

        //TODO Sql Injection
        var commandText = $"TRUNCATE TABLE \"{tableName}\"";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(commandText, connection);

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
            when (ex is NpgsqlException || ex is TaskCanceledException)
        {
            // Log or handle database-specific exceptions as needed.
            throw new InvalidOperationException($"Error truncating table {tableName}: {ex.Message}", ex);
        }
    }
}

