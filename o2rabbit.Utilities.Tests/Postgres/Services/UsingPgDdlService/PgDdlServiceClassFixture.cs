using Npgsql;
using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class PgDdlServiceClassFixture : IDisposable
{
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
        
        using var command = new NpgsqlCommand($"create table public.testTable(Id integer)", connection);
        
        command.ExecuteNonQuery();
        
        using var command2 = new NpgsqlCommand($"insert into public.testTable VALUES (1),(2)", connection);
        
        command2.ExecuteNonQuery();
    }

    public void Dispose()
    {
    }
}