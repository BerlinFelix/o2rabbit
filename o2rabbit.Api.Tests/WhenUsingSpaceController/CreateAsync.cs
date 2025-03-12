using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Spaces;
using o2rabbit.Api.Extensions;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingSpaceController;

public class CreateAsync
{
    [Fact]
    public async Task WhenCalled_CallsSpaceService()
    {
        var fixture = new Fixture();
        var newSpace = fixture.Create<NewSpaceCommand>();
        var spaceServiceMock = new Mock<ISpaceService>();
        var foundSpace = new Space() { Id = 2 };
        spaceServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundSpace))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);

        await sut.CreateAsync(newSpace);

        spaceServiceMock.Verify(m => m.CreateAsync(newSpace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsValidationNotSuccessfulError_ReturnsBadRequest()
    {
        var fixture = new Fixture();
        var newSpace = fixture.Create<NewSpaceCommand>();
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()));
        var sut = new SpaceController(spaceServiceMock.Object);

        var response = await sut.CreateAsync(newSpace);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsSuccess_ReturnsOkWithSpace()
    {
        var fixture = new Fixture();
        var newSpace = fixture.Create<NewSpaceCommand>();
        var spaceServiceMock = new Mock<ISpaceService>();
        var foundSpace = new Space() { Id = 2 };
        spaceServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundSpace));
        var sut = new SpaceController(spaceServiceMock.Object);

        var response = await sut.CreateAsync(newSpace);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().BeEquivalentTo(foundSpace.ToDefaultDto());
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsUnknownError_Returns500()
    {
        var fixture = new Fixture();
        var newSpace = fixture.Create<NewSpaceCommand>();
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        var sut = new SpaceController(spaceServiceMock.Object);

        var response = await sut.CreateAsync(newSpace);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}