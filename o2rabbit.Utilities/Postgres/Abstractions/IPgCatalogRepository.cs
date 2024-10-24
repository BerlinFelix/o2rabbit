namespace o2rabbit.Utilities.Postgres.Abstractions;

public interface IPgCatalogRepository
{
    /// <summary>
    /// Gets all tables in the database.
    /// </summary>
    /// <param name="connectionString">The connectionString to the database.</param>
    /// <param name="schemaName">The optional schemaname. If left blank returns all tables for all schemas except information_schema and pg_catalog.</param>
    /// <returns></returns>
    Task<IEnumerable<string>> GetTableNamesAsync(string connectionString, string? schemaName = null, CancellationToken cancellationToken = default);
}