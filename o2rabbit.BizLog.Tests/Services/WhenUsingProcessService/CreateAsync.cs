using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Services;
using o2rabbit.Migrations.Context;
using o2rabbit.Models;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class CreateAsync
{
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";
    private readonly Fixture _fixture;

    public CreateAsync()
    {
        _fixture = new Fixture();

        var container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();
        
        container.StartAsync().Wait();

        var connectionString = container.GetConnectionString();

        var migrationContext = new DefaultContext(connectionString);
        
        migrationContext.Database.Migrate();
        
        
    }


    [Fact]
    public async Task GivenNullInput_ReturnsError()
    {
        // Arrange
        // ProcessService processService = new ProcessService();
        // Process? process = null;
        //
        // // Act
        // var result = await processService.CreateAsync(process);
        //
        // // Assert
        // result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidInput_ReturnsProcess()
    {
    }
}