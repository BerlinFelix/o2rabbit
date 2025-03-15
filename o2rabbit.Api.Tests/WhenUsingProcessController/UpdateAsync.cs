using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Processes;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class UpdateAsync
{
    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var update = new UpdateProcessCommand();

        await sut.UpdateAsync(1, update);

        processServiceMock.VerifyAll();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsValidationNotSuccessfulError_ReturnsBadRequest()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var update = new UpdateProcessCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOkWithProcess()
    {
        var processServiceMock = new Mock<IProcessService>();
        var updatedProcess = new Process() { Name = "Updated" };
        processServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(updatedProcess))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var update = new UpdateProcessCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result!;
        objectResult.Value.Should().BeEquivalentTo(updatedProcess, config =>
            config.ExcludingMissingMembers());
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.UpdateAsync(It.IsAny<UpdateProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);
        var update = new UpdateProcessCommand();
        var response = await sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
}