using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers;
using o2rabbit.Api.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class UpdateAsync
{
    private readonly ProcessController _sut;
    private readonly Mock<IProcessService> _processServiceMock;
    private readonly IFixture _fixture;

    public UpdateAsync()
    {
        _processServiceMock = new Mock<IProcessService>();
        _processServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new ProcessController(_processServiceMock.Object);

        _fixture = new Fixture().Customize(new ProcessHasNoParentsAndNoChildren());
    }

    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var process = _fixture.Create<Process>();
        await _sut.UpdateAsync(process);

        _processServiceMock.Verify(m => m.UpdateAsync(process, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.UpdateAsync(process);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.UpdateAsync(process, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(process));

        var response = await _sut.UpdateAsync(process);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOkWithProcess()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.UpdateAsync(process, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(process));

        var response = await _sut.UpdateAsync(process);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result!;
        objectResult.Value.Should().Be(process);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.UpdateAsync(process);

        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
}