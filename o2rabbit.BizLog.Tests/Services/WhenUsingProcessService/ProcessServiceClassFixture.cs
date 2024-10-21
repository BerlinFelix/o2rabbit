using Microsoft.EntityFrameworkCore;
using o2rabbit.Migrations.Context;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class ProcessServiceClassFixture: IDisposable
{
    public static string? ConnectionString { get; private set; }
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";
    
    public ProcessServiceClassFixture()
    {
        var container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();

        container.StartAsync().Wait(TimeSpan.FromMinutes(1));
        ConnectionString = container.GetConnectionString();
        var migrationContext = new DefaultContext(ConnectionString);
        migrationContext.Database.Migrate();
    }
    public void Dispose()
    {
               
    }
}