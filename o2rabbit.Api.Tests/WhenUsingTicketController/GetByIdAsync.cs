using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class GetByIdAsync
{
    private readonly TicketController _sut;
    private readonly Fixture _fixture;
    private readonly Mock<ITicketService> _ticketServiceMock;

    public GetByIdAsync()
    {
        _ticketServiceMock = new Mock<ITicketService>();
        _ticketServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetTicketByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));
        _sut = new TicketController(_ticketServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Customize(new IgnoreRecursion());
    }

    [Fact]
    public async Task WhenCalled_CallsTicketService()
    {
        await _sut.GetByIdAsync(1);

        _ticketServiceMock.Verify(
            m => m.GetByIdAsync(1, It.IsAny<GetTicketByIdOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenTicketServiceReturnsInvalidIdError_ReturnsBadRequest()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetTicketByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()));

        var response = await _sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOk()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m =>
                m.GetByIdAsync(It.IsAny<long>(), It.IsAny<GetTicketByIdOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.GetByIdAsync(1);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOkWithTicket()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m =>
                m.GetByIdAsync(ticket.Id,
                    It.IsAny<GetTicketByIdOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.GetByIdAsync(ticket.Id);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result;
        objectResult.Value.Should().BeEquivalentTo(ticket.ToDefaultDto());
    }

    [Fact]
    public async Task WhenTicketServiceReturnsUnknownError_Returns500()
    {
        var ticket = _fixture.Create<Ticket>();
        _ticketServiceMock.Setup(m => m.GetByIdAsync(ticket.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.GetByIdAsync(ticket.Id);

        response.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)response.Result;
        objectResult.StatusCode.Should().Be(500);
    }
}