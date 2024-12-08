using AutoFixture;
using FluentAssertions;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Tests.Extensions;

public class TicketExtensions
{
    private readonly Fixture _fixture;

    public TicketExtensions()
    {
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoChildren());
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