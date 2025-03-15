using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Processes;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class DeleteAsync : IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;

    public DeleteAsync(ProcessServiceClassFixture classFixture)
    {
        ArgumentNullException.ThrowIfNull(classFixture);

        _classFixture = classFixture;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsFail(long id)
    {
        await SetUpDbAsync();
        var sut = SetUpDefaultSut();

        var result = await sut.DeleteAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsUnknownInvalidIdError(long id)
    {
        await SetUpDbAsync();
        var sut = SetUpDefaultSut();

        var result = await sut.DeleteAsync(id);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenExistingId_ReturnsOk()
    {
        await SetUpDbAsync();
        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingProcess = new Process { Id = 1 };
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GivenExistingId_DeletesProcess()
    {
        await SetUpDbAsync();
        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingProcess = new Process { Id = 1 };
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.DeleteAsync(1);

        await using var controlContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));

        var space = await controlContext.Processes.FindAsync((long)1);
        space.Should().BeNull();
    }

    private ProcessService SetUpDefaultSut()
    {
        var validatorMock = new Mock<IProcessValidator>();
        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
            { ConnectionString = _classFixture.ConnectionString }));
        var loggerMock = new Mock<ILogger<ProcessService>>();
        var sut = new ProcessService(context, loggerMock.Object, validatorMock.Object);
        return sut;
    }

    private async Task SetUpDbAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}