using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Spaces;
using o2rabbit.Api.Extensions;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingSpaceController;

public class GetByIdAsync
{
    [Fact]
    public async Task WhenCalled_CallsSpaceService()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetSpaceByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        spaceServiceMock.Verify(
            m => m.GetByIdAsync(1, It.IsAny<GetSpaceByIdOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsInvalidIdError_ReturnsNotFound()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetSpaceByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsSuccess_ReturnsOkWithSpace()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        var foundSpace = new Space() { Id = 2 };
        spaceServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetSpaceByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundSpace))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().BeEquivalentTo(foundSpace.ToDefaultDto());
    }

    [Fact]
    public async Task WhenSpaceServiceReturnsUnknownError_Returns500()
    {
        var spaceServiceMock = new Mock<ISpaceService>();
        spaceServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetSpaceByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new SpaceController(spaceServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}