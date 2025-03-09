using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Provides a container with all migrations added from Defaultcontext.
/// Initialization and disposal are async by implementing <see cref="IAsyncLifetime"/>.
/// </summary>
public class ProcessServiceClassFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    public string ConnectionString { get; private set; } = null!;
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();

        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
        await using var migrationContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = ConnectionString! }));
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}