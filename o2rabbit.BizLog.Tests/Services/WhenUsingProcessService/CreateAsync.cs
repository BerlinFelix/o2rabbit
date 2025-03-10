using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using ProcessService = o2rabbit.BizLog.Services.Processes.ProcessService;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class CreateAsync : IClassFixture<ProcessServiceClassFixture>, IAsyncLifetime
{
    private readonly ProcessServiceClassFixture _classFixture;
    private readonly Fixture _fixture;
    private readonly DefaultContext _context;
    private readonly Mock<ILogger<ProcessService>> _loggerMock;
    private readonly ProcessService _sut;

    public CreateAsync(ProcessServiceClassFixture classFixture)
    {
        _classFixture = classFixture;

        _fixture = new Fixture();
        _fixture.Customize(new ProcessHasNoParentsAndNoChildren());

        _context = new DefaultContext(
            new OptionsWrapper<DefaultContextOptions>(
                new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString ??
                                       throw new TypeInitializationException(nameof(_classFixture), null)
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
        var result = await _sut.CreateAsync(process!);

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

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var savedProcess = await context.Processes.FindAsync(result.Value.Id);

        savedProcess.Should().NotBeNull();
        savedProcess.Name.Should().Be(process.Name);
    }

    [Fact]
    public async Task IfAnyExceptionIsThrownWhenAccessingDb_ReturnsUnknownError()
    {
        var process = _fixture.Create<Process>();
        var contextMock = new Mock<DefaultContext>();
        contextMock.Setup(x => x.Processes).Throws<Exception>();
        var loggerMock = new Mock<ILogger<ProcessService>>();

        var sut = new ProcessService(contextMock.Object, loggerMock.Object);

        var result = await sut.CreateAsync(process);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
    }

    public async Task InitializeAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
    }
}