using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Spaces;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingSpaceController;

public class UpdateAsync
{
    [Fact]
    public async Task WhenCalled_CallsSpaceService()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var update = new UpdateSpaceCommand();
        var response = await sut.UpdateAsync(1, update);

        spaceServiceMock.VerifyAll();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsValidationNotSuccessfulError_ReturnsBadRequest()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var update = new UpdateSpaceCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsSuccess_ReturnsOkWithSpace()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        var updatedSpace = new Space() { Title = "Updated" };
        spaceServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(updatedSpace))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var update = new UpdateSpaceCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result!;
        objectResult.Value.Should().BeEquivalentTo(updatedSpace);
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsUnknownError_Returns500()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateSpaceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var update = new UpdateSpaceCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
}