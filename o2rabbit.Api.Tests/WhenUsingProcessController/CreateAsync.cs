using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Processes;
using o2rabbit.Api.Extensions;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class CreateAsync
{
    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var fixture = new Fixture();
        var newProcess = fixture.Create<NewProcessCommand>();
        var processServiceMock = new Mock<IProcessService>();
        var foundProcess = new Process() { Id = 2 };
        processServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundProcess))
            .Verifiable();
        var sut = new ProcessController(processServiceMock.Object);

        await sut.CreateAsync(newProcess);

        processServiceMock.Verify(m => m.CreateAsync(newProcess, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsValidationNotSuccessfulError_ReturnsBadRequest()
    {
        var fixture = new Fixture();
        var newProcess = fixture.Create<NewProcessCommand>();
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()));
        var sut = new ProcessController(processServiceMock.Object);

        var response = await sut.CreateAsync(newProcess);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOkWithProcess()
    {
        var fixture = new Fixture();
        var newProcess = fixture.Create<NewProcessCommand>();
        var processServiceMock = new Mock<IProcessService>();
        var foundProcess = new Process() { Id = 2 };
        processServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(foundProcess));
        var sut = new ProcessController(processServiceMock.Object);

        var response = await sut.CreateAsync(newProcess);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().BeEquivalentTo(foundProcess.ToDefaultDto());
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var fixture = new Fixture();
        var newProcess = fixture.Create<NewProcessCommand>();
        var processServiceMock = new Mock<IProcessService>();
        processServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<NewProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        var sut = new ProcessController(processServiceMock.Object);

        var response = await sut.CreateAsync(newProcess);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}