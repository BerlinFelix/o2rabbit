using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;
using Moq;
using Npgsql;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core;
using o2rabbit.Migrations.Context;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Postgres.Services;
using Testcontainers.PostgreSql;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class CreateAsync : IClassFixture<ProcessServiceClassFixture>, IDisposable
{
    private readonly Fixture _fixture;
    private readonly ProcessServiceContext _context;
    private readonly Mock<ILogger<ProcessService>> _loggerMock;
    private readonly ProcessService _sut;

    public CreateAsync()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ProcessHasNoParentsAndNoChildren());

        _context = new ProcessServiceContext(
            new OptionsWrapper<ProcessServiceContextOptions>(
                new ProcessServiceContextOptions()
                {
                    ConnectionString = ProcessServiceClassFixture.ConnectionString ??
                                       throw new TypeInitializationException(nameof(ProcessServiceClassFixture), null)
                }));

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

        var context = new DefaultContext(ProcessServiceClassFixture.ConnectionString);

        var savedProcess = await context.Processes.FindAsync(process.Id);

        savedProcess.Should().NotBeNull();
        savedProcess.Name.Should().Be(process.Name);
    }

    [Fact]
    public async Task IfAnyExceptionIsThrownWhenAccessingDb_ReturnsUnknownError()
    {
        var process = _fixture.Create<Process>();
        var contextMock = new Mock<ProcessServiceContext>();
        contextMock.Setup(x => x.Processes).Throws<Exception>();
        var loggerMock = new Mock<ILogger<ProcessService>>();
        
        var sut = new ProcessService(contextMock.Object, loggerMock.Object);
        
        var result = await sut.CreateAsync(process);
        
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
    }
    public void Dispose()
    {
        using var connection = new NpgsqlConnection(ProcessServiceClassFixture.ConnectionString);
        connection.Open();
        var pgCatalogRepository = new PgCatalogRepository();
        var exitingTables = pgCatalogRepository.GetAllTableNamesAsync((ProcessServiceClassFixture.ConnectionString!))
            .GetAwaiter().GetResult();
        foreach (var table in exitingTables)
        {
            using var command = new PgDdlService().GenerateTruncateTableCommand(table, connection);
            command.ExecuteNonQuery();
        }
    }
}