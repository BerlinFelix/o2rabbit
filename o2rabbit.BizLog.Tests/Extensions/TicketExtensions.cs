using AutoFixture;
using FluentAssertions;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;

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
        var newTicket = _fixture.Create<NewTicketDto>();

        var ticket = newTicket.ToTicket();

        ticket.Should().BeEquivalentTo(newTicket);
    }

    [Fact]
    public void Ticket_ToDefaultDto_IsEquivalent()
    {
        var ticket = _fixture.Create<Ticket>();

        var defaultDto = ticket.ToDefaultDto();

        defaultDto.Should().BeEquivalentTo(ticket, config =>
        {
            config.Excluding(t => t.Children)
                .Excluding(t => t.Parent)
                .Excluding(t => t.Process);
            return config;
        });
    }


    [Fact]
    public void Ticket_ToDefaultDto_HasCorrectChildrenIds()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasChildren());
        var ticket = _fixture.Create<Ticket>();

        var defaultDto = ticket.ToDefaultDto();

        defaultDto.ChildrenIds.Should().HaveCount(ticket.Children.Count);
    }
}