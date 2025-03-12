using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Spaces;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingSpaceController;

public class DeleteAsync
{
    [Fact]
    public async Task WhenCalled_CallsSpaceService()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        await sut.DeleteAsync(1);

        spaceServiceMock.Verify(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsSuccess_ReturnsOk()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsUnknownError_Returns500()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<ObjectResult>();
        response.As<ObjectResult>().StatusCode.Should().Be(500);
    }
}