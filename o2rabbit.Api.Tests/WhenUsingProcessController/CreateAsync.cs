using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers;
using o2rabbit.Api.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Tests.WhenUsingProcessController;

public class CreateAsync
{
    private readonly ProcessController _sut;
    private readonly Fixture _fixture;
    private readonly Mock<IProcessService> _processServiceMock;

    public CreateAsync()
    {
        _processServiceMock = new Mock<IProcessService>();
        _processServiceMock.Setup(m => m.CreateAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));
        _sut = new ProcessController(_processServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Customize(new IgnoreRecursion());
    }

    [Fact]
    public async Task WhenCalled_CallsProcessService()
    {
        var process = _fixture.Create<Process>();

        await _sut.CreateAsync(process);

        _processServiceMock.Verify(m => m.CreateAsync(process, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenProcessServiceReturnsError_ReturnsBadRequest()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.CreateAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Failed to create process"));

        var response = await _sut.CreateAsync(process);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenProcessServiceReturnsSuccess_ReturnsOk()
    {
        var process = _fixture.Create<Process>();
        _processServiceMock.Setup(m => m.CreateAsync(process, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(process));
        
        var response = await _sut.CreateAsync(process);
        
        response.Result.Should().BeOfType<OkObjectResult>();
    }
}