using Npgsql;
using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Postgres.Services;

public class PgCatalogRepositoryClassFixture : IDisposable
{
    public static List<string> ExistingSchemas { get; } = ["testschema1", "testschema2"];
    public static List<string> ExistingTables { get; } = ["table1", "table2"];
    public static string ConnectionString { get; private set; }

    private const string _USER = "testUser";
    private const string _PASSWORD = "password";


    public PgCatalogRepositoryClassFixture()
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
        foreach (var schema in ExistingSchemas)
        {
            using var command = new NpgsqlCommand($"create schema {schema}", connection);
            command.ExecuteNonQuery();
        }

        foreach (var table in ExistingTables)
        {
            using var command = new NpgsqlCommand($"create table {ExistingSchemas[0]}.{table} (Id int not null)", connection);
            command.ExecuteNonQuery();
        }
    }


    public void Dispose()
    {
    }
}