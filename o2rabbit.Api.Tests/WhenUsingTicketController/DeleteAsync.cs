using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class DeleteAsync
{
    private readonly TicketController _sut;
    private readonly Mock<ITicketService> _ticketServiceMock;

    public DeleteAsync()
    {
        _ticketServiceMock = new Mock<ITicketService>();
        _ticketServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new TicketController(_ticketServiceMock.Object);
    }

    [Fact]
    public async Task WhenCalled_CallsTicketService()
    {
        await _sut.DeleteAsync(1);

        _ticketServiceMock.Verify(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTicketServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        _ticketServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOk()
    {
        _ticketServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsUnknownError_Returns500()
    {
        _ticketServiceMock.Setup(m => m.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.DeleteAsync(1);

        response.Should().BeOfType<ObjectResult>();
        response.As<ObjectResult>().StatusCode.Should().Be(500);
    }
}