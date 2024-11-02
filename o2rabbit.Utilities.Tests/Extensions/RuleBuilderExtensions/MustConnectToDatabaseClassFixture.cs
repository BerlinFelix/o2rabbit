using Testcontainers.PostgreSql;

namespace o2rabbit.Utilities.Tests.Extensions.RuleBuilderExtensions;

public class MustConnectToDatabaseClassFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } = null!;
    private readonly PostgreSqlContainer _container;
    private const string _USER = "testuser";
    private const string _PASSWORD = "password";

    public MustConnectToDatabaseClassFixture()
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
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}