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

public class DeleteAsync
{
    private readonly ProcessController _sut;
    private readonly Fixture _fixture;
    private readonly Mock<IProcessService> _processServiceMock;

    public DeleteAsync()
    {
        _processServiceMock = new Mock<IProcessService>();
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new ProcessController(_processServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Customize(new IgnoreRecursion());
    }

    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var process = _fixture.Create<Process>();

        await _sut.DeleteAsync(1);

        _processServiceMock.Verify(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<ObjectResult>();
        response.As<ObjectResult>().StatusCode.Should().Be(500);
    }
}