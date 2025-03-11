using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Spaces;
using o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingSpaceService;

public class GetByIdAsync : IClassFixture<SpaceServiceClassFixture>
{
    private readonly SpaceServiceClassFixture _classFixture;

    public GetByIdAsync(SpaceServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private SpaceService SetUpDefaultSut()
    {
        var validatorMock = new Mock<ISpaceValidator>();
        var okValidationResult = new ValidationResult();
        validatorMock.Setup(m => m.ValidateNewSpace(It.IsAny<NewSpaceCommand>()))
            .Returns(okValidationResult);
        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
            { ConnectionString = _classFixture.ConnectionString }));
        var loggerMock = new Mock<ILogger<SpaceService>>();
        var sut = new SpaceService(context, loggerMock.Object, validatorMock.Object);
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
    public async Task GivenInvalidId_ReturnsFailed()
    {
        await SetUpDbAsync();
        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(-1);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenInvalidId_ReturnsInvalidIdError()
    {
        await SetUpDbAsync();
        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(-1);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenValidId_OkWithSpace()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingSpace = new Space { Id = 1 };
        setupContext.Add(existingSpace);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Space>();
        result.Value.Id.Should().Be(1);
    }


    [Fact]
    public async Task GivenCommentsRequested_ReturnsOkWithSpaceAndComments()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingSpace = new Space { Id = 1 };
        setupContext.Add(existingSpace);
        var existingComment = new SpaceComment
        {
            Id = 1,
            Text = "comment",
            SpaceId = 1,
        };
        setupContext.Add(existingComment);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1, new GetSpaceByIdOptions() { IncludeComments = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Comments.Should().ContainSingle(c => c.Id == 1);
    }

    [Fact]
    public async Task GivenTicketsRequested_ReturnsOkWithSpaceAndTickets()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingSpace = new Space { Id = 1 };
        setupContext.Add(existingSpace);
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
        existingProcess.PossibleSpaces.Add(existingSpace);
        setupContext.Add(existingProcess);
        setupContext.Add(existingTicket);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1, new GetSpaceByIdOptions() { IncludeTickets = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.AttachedTickets.Should().ContainSingle(p => p.Id == 1);
    }

    [Fact]
    public async Task GivenProcessesRequested_ReturnsOkWithSpaceAndProcesses()
    {
        await SetUpDbAsync();

        await using var setupContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var existingSpace = new Space { Id = 1 };
        setupContext.Add(existingSpace);
        var existingProcess = new Process()
        {
            Id = 1,
            Name = "Test Process",
        };
        existingProcess.PossibleSpaces.Add(existingSpace);
        setupContext.Add(existingProcess);
        await setupContext.SaveChangesAsync();

        var sut = SetUpDefaultSut();

        var result = await sut.GetByIdAsync(1, new GetSpaceByIdOptions() { IncludeProcesses = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.AttachableProcesses.Should().ContainSingle(p => p.Id == 1);
    }
}