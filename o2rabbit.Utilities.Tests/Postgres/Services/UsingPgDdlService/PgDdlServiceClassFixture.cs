using Npgsql;
using o2rabbit.Utilities.Postgres.Models;
using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class PgDdlServiceClassFixture : IDisposable
{
    public static readonly QualifiedTableName TableName = new QualifiedTableName("public", "testTable");

    //TODO remove static connectionstrings and replace with lifetimes
    public static string ConnectionString { get; private set; }

    private const string _USER = "testUser";
    private const string _PASSWORD = "password";


    public PgDdlServiceClassFixture()
    {
        var container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();

        container.StartAsync().Wait(TimeSpan.FromMinutes(1));

        ConnectionString = container.GetConnectionString();

        using var connection = new NpgsqlConnection(ConnectionString);

        connection.Open();

        using var command = new NpgsqlCommand($"create table {TableName}(Id integer)", connection);

        command.ExecuteNonQuery();

        using var command2 = new NpgsqlCommand($"insert into {TableName}VALUES (1),(2)", connection);

        command2.ExecuteNonQuery();
    }

    public void Dispose()
    {
    }
}