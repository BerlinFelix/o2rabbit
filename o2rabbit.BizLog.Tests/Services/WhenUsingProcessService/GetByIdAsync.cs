using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Processes;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingProcessService;

public class GetByIdAsync : IClassFixture<ProcessServiceClassFixture>
{
    private readonly ProcessServiceClassFixture _classFixture;

    public GetByIdAsync(ProcessServiceClassFixture classFixture)
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
    public async Task GivenInvalidId_ReturnsFailWithInvalidIdError()
    {
        await SetUpDbAsync();
        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(-1);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenValidId_OkWithProcess()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingProcess = new Process { Id = 1 };
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Process>();
        result.Value.Id.Should().Be(1);
    }


    [Fact]
    public async Task GivenCommentsRequested_ReturnsOkWithProcessAndComments()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingProcess = new Process { Id = 1 };
        setupContext.Add(existingProcess);
        var existingComment = new ProcessComment
        {
            Id = 1,
            Text = "comment",
            ProcessId = 1,
        };
        setupContext.Add(existingComment);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1, new GetProcessByIdOptions() { IncludeComments = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Comments.Should().ContainSingle(c => c.Id == 1);
    }

    [Fact]
    public async Task GivenTicketsRequested_ReturnsOkWithProcessAndTickets()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingSpace = new Space { Id = 1 };
        var existingProcess = new Process()
        {
            Id = 1,
            Name = "Test Process",
        };
        var existingTicket = new Ticket
        {
            Id = 1,
            Name = "name",
            ProcessId = 1,
            SpaceId = 1,
        };
        existingProcess.Tickets.Add(existingTicket);
        setupContext.Add(existingSpace);
        setupContext.Add(existingProcess);
        setupContext.Add(existingTicket);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1, new GetProcessByIdOptions() { IncludeTickets = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Tickets.Should().ContainSingle(p => p.Id == 1);
    }
}