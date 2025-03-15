using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Processes;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class UpdateAsync : IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;

    public UpdateAsync(ProcessServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private ProcessService SetUpDefaultSut()
    {
        var validatorMock = new Mock<IProcessValidator>();
        var okValidationResult = new ValidationResult();
        validatorMock.Setup(m => m.ValidateUpdatedProcess(It.IsAny<UpdateProcessCommand>()))
            .Returns(okValidationResult);
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

    [Fact]
    public async Task GivenInvalidCommand_ReturnsFailed()
    {
        await SetUpDbAsync();

        var command = new UpdateProcessCommand()
        {
            Id = 1,
            Name = "invalid",
            Description = ""
        };
        var fixture = new AutoMoqFixture();
        var validatorMock = fixture.Freeze<Mock<IProcessValidator>>();
        var failedValidationResult = new ValidationResult() { Errors = [new ValidationFailure("title", "error")] };
        validatorMock.Setup(m => m.ValidateUpdatedProcess(It.IsAny<UpdateProcessCommand>()))
            .Returns(failedValidationResult);
        var sut = fixture.Create<ProcessService>();

        var result = await sut.UpdateAsync(command);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidCommand_ReturnsOkWithUpdatedProcess()
    {
        await SetUpDbAsync();
        var existingProcess = new Process { Id = 1 };
        await using var setupContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();
        var command = new UpdateProcessCommand()
        {
            Id = 1,
            Name = "valid",
            Description = "valid"
        };

        var sut = SetUpDefaultSut();
        var result = await sut.UpdateAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(command);
    }

    [Fact]
    public async Task GivenValidCommand_UpdatesProcessInDb()
    {
        await SetUpDbAsync();
        var existingProcess = new Process { Id = 1, Name = "oldName" };
        await using var setupContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();
        var command = new UpdateProcessCommand()
        {
            Id = 1,
            Name = "valid",
            Description = "valid"
        };

        var sut = SetUpDefaultSut();
        var result = await sut.UpdateAsync(command);

        await using var controlContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var space = await controlContext.Processes.FindAsync(command.Id);
        space.Should().NotBeNull();
        space.Should().BeEquivalentTo(command);
    }
}