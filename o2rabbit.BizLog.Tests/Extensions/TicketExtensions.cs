using AutoFixture;
using FluentAssertions;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;

namespace o2rabbit.BizLog.Tests.Extensions;

public class TicketExtensions
{
    private readonly Fixture _fixture;

    public TicketExtensions()
    {
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoChildren());
    }

    [Fact]
    public void NewTicketDto_ToTicket_IsEquivalentToDto()
    {
        var newTicket = _fixture.Create<NewTicketCommand>();

        var ticket = newTicket.ToTicket();

        ticket.Should().BeEquivalentTo(newTicket);
    }
}