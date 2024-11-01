using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class DeleteAsync
{
    private readonly ProcessController _sut;
    private readonly Mock<IProcessService> _processServiceMock;

    public DeleteAsync()
    {
        _processServiceMock = new Mock<IProcessService>();
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new ProcessController(_processServiceMock.Object);
    }

    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        await _sut.DeleteAsync(1);

        _processServiceMock.Verify(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<ObjectResult>();
        response.As<ObjectResult>().StatusCode.Should().Be(500);
    }
}