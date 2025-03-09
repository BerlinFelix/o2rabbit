using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

public class DeleteAsync : IAsyncLifetime, IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly ProcessService _sut;
    private readonly DefaultContext _processContext;

    public DeleteAsync(ProcessServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        _fixture = new Fixture();
        _fixture.Customize(new ProcessHasNoParentsAndNoChildren());

        _processContext =
            new DefaultContext(
                new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<ProcessService>>();
        _sut = new ProcessService(_processContext, loggerMock.Object);
    }

    public async Task InitializeAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureCreatedAsync();
        var existingProcess = _fixture.Create<Process>();
        var existingProcess2 = _fixture.Create<Process>();
        existingProcess.Id = 1;
        existingProcess2.Id = 2;

        _defaultContext.Add(existingProcess);
        _defaultContext.Add(existingProcess2);

        await _defaultContext.SaveChangesAsync();

        _defaultContext.Entry(existingProcess).State = EntityState.Detached;
        _defaultContext.Entry(existingProcess2).State = EntityState.Detached;
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsFail(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsUnknownInvalidIdError(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsOk(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_DeletesProcess(long id)
    {
        var result = await _sut.DeleteAsync(id);

        var process = await _defaultContext.Processes.FindAsync(id);
        process.Should().BeNull();
    }

    public async Task DisposeAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await _defaultContext.DisposeAsync();
    }
}