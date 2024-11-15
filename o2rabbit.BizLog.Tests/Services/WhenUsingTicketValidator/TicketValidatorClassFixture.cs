using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketValidator;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Provides a container with all migrations added from Defaultcontext.
/// Initialization and disposal are async by implementing <see cref="IAsyncLifetime"/>.
/// </summary>
public class TicketValidatorClassFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer? _container;
    public string ConnectionString { get; private set; } = null!;
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";

    public TicketValidatorClassFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("Default")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
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