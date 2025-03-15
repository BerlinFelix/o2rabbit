using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Processes;
using o2rabbit.Api.Extensions;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class GetByIdAsync
{
    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetProcessByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        processServiceMock.Verify(
            m => m.GetByIdAsync(1, It.IsAny<GetProcessByIdOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsNotFound()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetProcessByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOkWithProcess()
    {
        var processServiceMock = new Mock<IProcessService>();
        var foundProcess = new Process() { Id = 2 };
        processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetProcessByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundProcess))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().BeEquivalentTo(foundProcess.ToDefaultDto());
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetProcessByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var response = await sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}