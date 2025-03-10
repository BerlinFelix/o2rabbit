using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;

public class SpaceServiceClassFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    internal string ConnectionString { get; set; } = string.Empty;
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