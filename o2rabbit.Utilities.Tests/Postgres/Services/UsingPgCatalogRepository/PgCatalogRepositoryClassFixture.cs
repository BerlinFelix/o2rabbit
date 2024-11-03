using Npgsql;
using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgCatalogRepository;

public class PgCatalogRepositoryClassFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public static List<string> ExistingSchemas { get; } = ["testschema1", "testschema2"];
    public static List<string> ExistingTables { get; } = ["\"testschema1\".\"table1\"", "\"testschema1\".\"table2\""];
    public string ConnectionString { get; private set; }

    private const string _USER = "testUser";
    private const string _PASSWORD = "password";


    public PgCatalogRepositoryClassFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();

        await using var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();
        foreach (var schema in ExistingSchemas)
        {
            await using var command = new NpgsqlCommand($"create schema {schema}", connection);
            await command.ExecuteNonQueryAsync();
        }

        foreach (var table in ExistingTables)
        {
            await using var command = new NpgsqlCommand($"create table {table} (Id int not null)", connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}