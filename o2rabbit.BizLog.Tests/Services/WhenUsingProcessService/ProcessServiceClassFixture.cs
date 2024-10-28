using Microsoft.EntityFrameworkCore;
using o2rabbit.Migrations.Context;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

// ReSharper disable once ClassNeverInstantiated.Global
public class ProcessServiceClassFixture : IAsyncLifetime 
{
    public static string? ConnectionString { get; private set; }
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";

    public async Task InitializeAsync()
    {
        var container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();

        await container.StartAsync();
        ConnectionString = container.GetConnectionString();
        var migrationContext = new DefaultContext(ConnectionString);
        await migrationContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;   
    }
}