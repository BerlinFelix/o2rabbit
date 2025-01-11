using AutoFixture;
using FluentAssertions;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;

namespace o2rabbit.BizLog.Tests.Extensions;

public class CommentExtensions
{
    private readonly Fixture _fixture;

    public CommentExtensions()
    {
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoParentsAndNoChildren());
    }

    [Fact]
    public void NewCommentCommand_ToDto_ShouldBeEquivalent()
    {
        var command = _fixture.Create<NewCommentCommand>();

        var ticket = command.ToComment();

        ticket.Should().BeEquivalentTo(command);
    }
}