using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets.UpdatedTicketDtoCustomizations;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
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
        _ticketServiceMock.Setup(m => m.UpdateAsync(It.IsAny<UpdateTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        _sut = new TicketController(_ticketServiceMock.Object);

        _fixture = new Fixture().Customize(new UpdatedTicketHasNoParentAndNoProcess());
    }

    [Fact]
    public async Task WhenCalled_CallsTicketService()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        await _sut.UpdateAsync(1, update);

        _ticketServiceMock.Verify(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTicketServiceReturnsValidationNotSuccessfulError_ReturnsBadRequest()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()));

        var response = await _sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOk()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        var ticket = new Ticket()
        {
            Id = update.Id,
            Name = update.Name,
            ParentId = update.ParentId,
        };
        _ticketServiceMock.Setup(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.UpdateAsync(1, update);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task WhenTicketServiceReturnsSuccess_ReturnsOkWithTicket()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        var ticket = new Ticket()
        {
            Id = update.Id,
            Name = update.Name,
            ParentId = update.ParentId,
        };
        _ticketServiceMock.Setup(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var response = await _sut.UpdateAsync(update.Id, update);

        response.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = (OkObjectResult)response.Result!;
        objectResult.Value.Should().BeEquivalentTo(ticket.ToDefaultDto());
    }

    [Fact]
    public async Task WhenTicketServiceReturnsUnknownError_Returns500()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        _ticketServiceMock.Setup(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));

        var response = await _sut.UpdateAsync(update.Id, update);

        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task EnsuresTicketIdIsReadFromQueryParam()
    {
        var update = _fixture.Create<UpdateTicketCommand>();
        var ticket = new Ticket()
        {
            Id = update.Id,
            Name = update.Name,
            ParentId = update.ParentId,
        };
        _ticketServiceMock.Setup(m => m.UpdateAsync(update, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ticket));

        var queryParamId = update.Id + 1;
        await _sut.UpdateAsync(queryParamId, update);

        update.Id.Should().Be(queryParamId);
    }
}