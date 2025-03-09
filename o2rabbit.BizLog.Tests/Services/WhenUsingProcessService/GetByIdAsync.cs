using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;
using ProcessService = o2rabbit.BizLog.Services.Processes.ProcessService;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class GetByIdAsync : IAsyncLifetime, IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly ProcessService _sut;

    public GetByIdAsync(ProcessServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(_classFixture.ConnectionString);
        _fixture = new Fixture();
        _fixture.Customize(new ProcessHasNoParentsAndNoChildren());

        var processContext =
            new ProcessServiceContext(
                new OptionsWrapper<ProcessServiceContextOptions>(new ProcessServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<ProcessService>>();
        _sut = new ProcessService(processContext, loggerMock.Object);
    }

    public async Task InitializeAsync()
    {
        await using var context = new DefaultContext(_classFixture.ConnectionString);
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
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccess(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccessWithProcess(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Process>();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdAndIncludeChildren_ReturnsIsSuccessWithChildren(long id)
    {
        var processWithParent = _fixture.Create<Process>();
        processWithParent.Id = 3;
        processWithParent.ParentId = id;

        _defaultContext.Add(processWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id, new GetProcessByIdOptions() { IncludeChildren = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Process>();
        result.Value.Children.Should().Contain(p => p.Id == 3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdWithChildren_ReturnsProcessWithoutChildren(long id)
    {
        var processWithParent = _fixture.Create<Process>();
        processWithParent.Id = 3;
        processWithParent.ParentId = id;

        _defaultContext.Add(processWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Process>();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdWithChildrenAndOptionsIncludingChildren_ReturnsProcessWithChildren(long id)
    {
        var processWithParent = _fixture.Create<Process>();
        processWithParent.ParentId = id;

        _defaultContext.Add(processWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id, new GetProcessByIdOptions() { IncludeChildren = true });

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Process>();
        // Note that you cannot use ...Should().BeEquivalentTo(), weil Parent-Child eine zirkulaere Referenz enthaelt.
        result.Value.Id.Should().Be(id);
        result.Value.Children.Should().HaveCount(1);
        result.Value.Children.First().ParentId.Should().Be(id);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsIsFail(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsInvalidIdError(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.Errors.Should().Contain(e => e is InvalidIdError);
    }

    public async Task DisposeAsync()
    {
        await using var context = new DefaultContext(_classFixture.ConnectionString);
        await context.Database.EnsureDeletedAsync();

        await _defaultContext.DisposeAsync();
    }
}