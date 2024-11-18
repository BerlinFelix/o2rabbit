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

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class UpdateAsync
{
    private readonly TicketController _sut;
    private readonly Mock<ITicketService> _ticketServiceMock;
    private readonly IFixture _fixture;

    public UpdateAsync()
    {
        _ticketServiceMock = new Mock<ITicketService>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new TicketController(_ticketServiceMock.Object);

        _fixture = new Fixture().Customize(new TicketHasNoParentsAndNoChildren());
    }

    [Fact]
    public async Task WhenCalled_CallsTicketService()
    {
        var ticket = _fixture.Create<Ticket>();
        await _sut.UpdateAsync(ticket);

        _ticketServiceMock.Verify(m => m.UpdateAsync(ticket, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTicketServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.UpdateAsync(ticket);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOk()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.UpdateAsync(ticket);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOkWithTicket()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.UpdateAsync(ticket);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result!;
        objectResult.Value.Should().Be(ticket);
    }

    [Fact]
    public async Task WhenTicketServiceReturnsUnknownError_Returns500()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.UpdateAsync(ticket);

        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
}