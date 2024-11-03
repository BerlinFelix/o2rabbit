using Npgsql;
using o2rabbit.Utilities.Postgres.Models;
using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class PgDdlServiceClassFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public static readonly QualifiedTableName TableName = new QualifiedTableName("public", "testTable");

    public string ConnectionString { get; private set; }

    private const string _USER = "testUser";
    private const string _PASSWORD = "password";


    public PgDdlServiceClassFixture()
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

        await using var command = new NpgsqlCommand($"create table {TableName}(Id integer)", connection);

        await command.ExecuteNonQueryAsync();

        await using var command2 = new NpgsqlCommand($"insert into {TableName}VALUES (1),(2)", connection);

        await command2.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}