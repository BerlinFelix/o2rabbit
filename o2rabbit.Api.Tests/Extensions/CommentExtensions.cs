using AutoFixture;
using FluentAssertions;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Tests.Extensions;

public class CommentExtensions
{
    private readonly Fixture _fixture;

    public CommentExtensions()
    {
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoParentsAndNoChildren());
    }

    [Fact]
    public void Comment_ToDto_ShouldBeEquivalent()
    {
        var comment = _fixture.Create<TicketComment>();

        var dto = comment.ToDto();

        dto.Should().BeEquivalentTo(comment, config => config
            .Excluding(x => x.Ticket));
    }
}