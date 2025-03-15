using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Processes;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class DeleteAsync
{
    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        await sut.DeleteAsync(1);

        processServiceMock.Verify(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.DeleteAsync(1);

        response.Should().BeOfType<ObjectResult>();
        response.As<ObjectResult>().StatusCode.Should().Be(500);
    }
}