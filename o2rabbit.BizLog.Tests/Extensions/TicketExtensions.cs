using AutoFixture;
using FluentAssertions;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Extensions;

namespace o2rabbit.BizLog.Tests.Extensions;

public class TicketExtensions
{
    private readonly Fixture _fixture;

    public TicketExtensions()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void NewTicketDto_ToTicket_IsEquivalentToDto()
    {
        var newTicket = _fixture.Create<NewTicketDto>();

        var ticket = newTicket.ToTicket();

        ticket.Should().BeEquivalentTo(newTicket);
    }
}