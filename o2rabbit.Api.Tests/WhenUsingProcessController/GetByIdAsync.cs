using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers;
using o2rabbit.Api.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class GetByIdAsync
{
    private readonly ProcessController _sut;
    private readonly Fixture _fixture;
    private readonly Mock<IProcessService> _processServiceMock;

    public GetByIdAsync()
    {
        _processServiceMock = new Mock<IProcessService>();
        _processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));
        _sut = new ProcessController(_processServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Customize(new IgnoreRecursion());
    }

    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        await _sut.GetByIdAsync(1);

        _processServiceMock.Verify(m => m.GetByIdAsync(1, It.IsAny<GetByIdOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(process));

        var response = await _sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOkWithProcess()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.GetByIdAsync(process.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(process));

        var response = await _sut.GetByIdAsync(process.Id);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().Be(process);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsUnknownError_Returns500()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.GetByIdAsync(process.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.GetByIdAsync(process.Id);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}