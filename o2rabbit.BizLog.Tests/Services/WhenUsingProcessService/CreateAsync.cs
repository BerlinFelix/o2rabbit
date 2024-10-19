using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core;
using o2rabbit.Migrations.Context;
using o2rabbit.Core;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class CreateAsync
{
    private const string _USER = "testUser";
    private const string _PASSWORD = "password";
    private readonly Fixture _fixture;
    private readonly ProcessServiceContext _context;
    private readonly Mock<ILogger<ProcessService>> _loggerMock;
    private readonly ProcessService _sut;
    private string? _connectionString;

    public CreateAsync()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ProcessHasNoParentsAndNoChildren());
        
        var container = new PostgreSqlBuilder()
            .WithDatabase("Processes")
            .WithUsername(_USER)
            .WithPassword(_PASSWORD)
            .Build();

        container.StartAsync().Wait();
        _connectionString = container.GetConnectionString();
        var migrationContext = new DefaultContext(_connectionString);
        migrationContext.Database.Migrate();

        _context = new ProcessServiceContext(
            new OptionsWrapper<ProcessServiceContextOptions>(
                new ProcessServiceContextOptions() { ConnectionString = _connectionString }));

        _loggerMock = new Mock<ILogger<ProcessService>>();

        _sut = new ProcessService(_context, _loggerMock.Object);
    }


    [Fact]
    public async Task GivenNullInput_ReturnsError()
    {
        // Arrange
        Process? process = null;

        // Act
        var result = await _sut.CreateAsync(process);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenInputWithExistingId_ReturnsInvalidIdError()
    {
        var existingProcess = _fixture.Create<Process>();
        await _context.AddAsync(existingProcess);
        await _context.SaveChangesAsync();

        var process = _fixture.Create<Process>();
        process.Id = existingProcess.Id;

        var result = await _sut.CreateAsync(process);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is InvalidIdError);
    }

    [Fact]
    public async Task GivenNewProcess_ReturnsOk()
    {
        var process = _fixture.Create<Process>();

        var result = await _sut.CreateAsync(process);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GivenNewProcess_ReturnsOkWithProcessAsValue()
    {
        var process = _fixture.Create<Process>();

        var result = await _sut.CreateAsync(process);

        result.Value.Should().BeOfType<Process>();
        result.Value.Should().Be(process);
    }

    [Fact]
    public async Task GivenNewProcess_CreatesNewProcessInDatabase()
    {
        var process = _fixture.Create<Process>();
        process.Id = 0;
        var result = await _sut.CreateAsync(process);

        var context = new DefaultContext(_connectionString!);

        var savedProcess = await context.Processes.FindAsync(process.Id);

        savedProcess.Should().NotBeNull();
        savedProcess.Name.Should().Be(process.Name);
    }
}