using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Spaces;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingSpaceService;

public class UpdateAsync : IClassFixture<SpaceServiceClassFixture>
{
    private readonly SpaceServiceClassFixture _classFixture;

    public UpdateAsync(SpaceServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private SpaceService SetUpDefaultSut()
    {
        var validatorMock = new Mock<ISpaceValidator>();
        var okValidationResult = new ValidationResult();
        validatorMock.Setup(m => m.ValidateUpdatedSpace(It.IsAny<UpdateSpaceCommand>()))
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
    public async Task GivenInvalidCommand_ReturnsFailed()
    {
        await SetUpDbAsync();

        var command = new UpdateSpaceCommand()
        {
            Id = 1,
            Title = "invalid",
            Description = ""
        };
        var fixture = new AutoMoqFixture();
        var validatorMock = fixture.Freeze<Mock<ISpaceValidator>>();
        var failedValidationResult = new ValidationResult() { Errors = [new ValidationFailure("title", "error")] };
        validatorMock.Setup(m => m.ValidateUpdatedSpace(It.IsAny<UpdateSpaceCommand>()))
            .Returns(failedValidationResult);
        var sut = fixture.Create<SpaceService>();

        var result = await sut.UpdateAsync(command);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidCommand_ReturnsOkWithUpdatedSpace()
    {
        await SetUpDbAsync();
        var existingSpace = new Space { Id = 1 };
        await using var setupContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        setupContext.Add(existingSpace);
        await setupContext.SaveChangesAsync();
        var command = new UpdateSpaceCommand()
        {
            Id = 1,
            Title = "valid",
            Description = "valid"
        };

        var sut = SetUpDefaultSut();
        var result = await sut.UpdateAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(command);
    }

    [Fact]
    public async Task GivenValidCommand_StoresSpaceInDb()
    {
        await SetUpDbAsync();
        var existingSpace = new Space { Id = 1 };
        await using var setupContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        setupContext.Add(existingSpace);
        await setupContext.SaveChangesAsync();
        var command = new UpdateSpaceCommand()
        {
            Id = 1,
            Title = "valid",
            Description = "valid"
        };

        var sut = SetUpDefaultSut();
        var result = await sut.UpdateAsync(command);

        await using var controlContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var space = await controlContext.Spaces.FindAsync(command.Id);
        space.Should().NotBeNull();
        space.Should().BeEquivalentTo(command);
    }
}