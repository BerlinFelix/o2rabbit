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
                .Excluding(t => t.Process)
                .Excluding(t => t.ParentId);
            return config;
        });
    }


    [Fact]
    public void Ticket_ToDefaultDto_HasCorrectChildren()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasChildren());
        var ticket = _fixture.Create<Ticket>();

        var defaultDto = ticket.ToDefaultDto();

        defaultDto.Children.Should().HaveCount(ticket.Children.Count);
    }

    [Fact]
    public void Ticket_ToChildDto_IsEquivalent()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var ticket = _fixture.Create<Ticket>();

        var dto = ticket.ToTinyTicketDto();


        dto.Should().BeEquivalentTo(ticket, config =>
        {
            config.Excluding(t => t.Children)
                .Excluding(t => t.Process)
                .Excluding((t => t.ProcessId))
                .Excluding(t => t.Children)
                .Excluding(t => t.Parent)
                .Excluding(t => t.ParentId)
                .Excluding(t => t.Comments);
            return config;
        });
    }
}