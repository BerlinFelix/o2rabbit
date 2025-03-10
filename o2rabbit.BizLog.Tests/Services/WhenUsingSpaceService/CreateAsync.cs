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

public class CreateAsync : IClassFixture<SpaceServiceClassFixture>
{
    private readonly SpaceServiceClassFixture _classFixture;

    public CreateAsync(SpaceServiceClassFixture classFixture)
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
    public async Task GivenInvalidCommand_ReturnsFailed()
    {
        await SetUpDbAsync();

        var command = new NewSpaceCommand
        {
            Title = "title",
            Description = "description"
        };
        var fixture = new AutoMoqFixture();
        var validatorMock = fixture.Freeze<Mock<ISpaceValidator>>();
        var failedValidationResult = new ValidationResult() { Errors = [new ValidationFailure("title", "error")] };
        validatorMock.Setup(m => m.ValidateNewSpace(It.IsAny<NewSpaceCommand>()))
            .Returns(failedValidationResult);
        var sut = fixture.Create<SpaceService>();

        var result = await sut.CreateAsync(command);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidCommand_ReturnsOkWithSpaceAndNewId()
    {
        await SetUpDbAsync();

        var command = new NewSpaceCommand
        {
            Title = "title",
            Description = "description"
        };
        var sut = SetUpDefaultSut();

        var result = await sut.CreateAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Space>();
        result.Value.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GivenValidCommand_StoresSpaceInDb()
    {
        await SetUpDbAsync();

        var command = new NewSpaceCommand
        {
            Title = "title",
            Description = "description"
        };
        var sut = SetUpDefaultSut();

        var result = await sut.CreateAsync(command);

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));

        var space = await context.Spaces.FindAsync(result.Value.Id);
        space.Should().NotBeNull();
        space.Should().BeEquivalentTo(command);
    }
}