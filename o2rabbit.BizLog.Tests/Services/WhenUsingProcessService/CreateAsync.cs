using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Processes;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class CreateAsync : IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;

    public CreateAsync(ProcessServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private ProcessService SetUpDefaultSut()
    {
        var validatorMock = new Mock<IProcessValidator>();
        var okValidationResult = new ValidationResult();
        validatorMock.Setup(m => m.ValidateNewProcess(It.IsAny<NewProcessCommand>()))
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

        var command = new NewProcessCommand
        {
            Name = "invalid",
            Description = "description",
            Workflow = new NewProcessCommand.NewWorkflowCommand(),
        };
        var validatorMock = new Mock<IProcessValidator>();
        var failedValidationResult = new ValidationResult() { Errors = { new ValidationFailure("title", "error") } };
        validatorMock.Setup(m => m.ValidateNewProcess(It.IsAny<NewProcessCommand>()))
            .Returns(failedValidationResult);
        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
            { ConnectionString = _classFixture.ConnectionString }));
        var loggerMock = new Mock<ILogger<ProcessService>>();
        var sut = new ProcessService(context, loggerMock.Object, validatorMock.Object);

        var result = await sut.CreateAsync(command);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidCommand_ReturnsOkWithProcessAndNewId()
    {
        await SetUpDbAsync();

        var command = new NewProcessCommand
        {
            Name = "title",
            Description = "description",
            Workflow = new NewProcessCommand.NewWorkflowCommand()
        };
        var sut = SetUpDefaultSut();

        var result = await sut.CreateAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Process>();
        result.Value.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GivenValidCommand_StoresProcessInDb()
    {
        await SetUpDbAsync();

        var command = new NewProcessCommand
        {
            Name = "title",
            Description = "description",
            Workflow = new NewProcessCommand.NewWorkflowCommand()
            {
                Statuses =
                {
                    new NewProcessCommand.NewStatusCommand { Name = "status1" },
                    new NewProcessCommand.NewStatusCommand { Name = "status2" }
                },
                StatusTransitions =
                {
                    new NewProcessCommand.NewStatusTransitionCommand
                        { Name = "default", FromStatusName = "status1", ToStatusName = "status2" }
                }
            }
        };
        var sut = SetUpDefaultSut();

        var result = await sut.CreateAsync(command);

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var process = await context.Processes.FindAsync(result.Value.Id);
        process.Should().NotBeNull();
        process.Should().BeEquivalentTo(command, config =>
        {
            config.Excluding(c => c.PossibleSpaceIds)
                .Excluding(c => c.SubProcessIds)
                .Excluding(c => c.Workflow);

            return config;
        });
    }

    [Fact]
    public async Task GivenValidCommand_StoresWorkflowInDb()
    {
        await SetUpDbAsync();

        var command = new NewProcessCommand
        {
            Name = "title",
            Description = "description",
            Workflow = new NewProcessCommand.NewWorkflowCommand()
            {
                Statuses =
                {
                    new NewProcessCommand.NewStatusCommand { Name = "status1" },
                    new NewProcessCommand.NewStatusCommand { Name = "status2" }
                },
                StatusTransitions =
                {
                    new NewProcessCommand.NewStatusTransitionCommand
                        { Name = "default", FromStatusName = "status1", ToStatusName = "status2" }
                }
            }
        };
        var sut = SetUpDefaultSut();

        var result = await sut.CreateAsync(command);

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var process = await context.Processes
            .Include(p => p.Workflow)
            .ThenInclude(workflow => workflow!.Statuses)
            .ThenInclude(status => status.FromTransitions)
            .Include(p => p.Workflow)
            .ThenInclude(workflow => workflow!.Statuses)
            .ThenInclude(status => status.ToTransitions)
            .SingleOrDefaultAsync();

        process.Should().NotBeNull();
        using var scope = new AssertionScope();
        process.Workflow!.Statuses.Should().HaveCount(2);
        process.Workflow!.Statuses.Should().BeEquivalentTo(command.Workflow.Statuses);
        process.Workflow!.Statuses.SelectMany(s => s.FromTransitions).Should().HaveCount(1);
        process.Workflow!.Statuses.First().FromTransitions!.First().FromStatus!.Id.Should().Be(1);
        process.Workflow!.Statuses.First().FromTransitions!.First().ToStatus!.Id.Should().Be(2);
        process.Workflow!.Statuses.SelectMany(s => s.ToTransitions).Should().HaveCount(1);
    }
}