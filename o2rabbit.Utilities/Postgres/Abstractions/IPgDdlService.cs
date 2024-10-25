using Npgsql;

namespace o2rabbit.Utilities.Postgres.Abstractions;
/// <summary>
/// Provides functionality for Postgres data definition language.
/// </summary>
public interface IPgDdlService
{
    /// <summary>
    /// Generates a truncate table statement.
    /// </summary>
    /// <param name="schemaName"></param>
    /// <param name="tableName"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public NpgsqlCommand GenerateTruncateTableCommand(string schemaName, string tableName, string? connectionString = null);

    /// <summary>
    /// Generate a random unique Sql parameter name prefixed with '@'.
    /// </summary>
    /// <returns></returns>
    public string GenerateSqlParameterAlias();
}