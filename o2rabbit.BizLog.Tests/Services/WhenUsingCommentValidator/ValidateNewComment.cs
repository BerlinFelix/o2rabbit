using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.CommentServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;

public class ValidateNewComment : IClassFixture<CommentServiceClassFixture>
{
    private readonly CommentServiceClassFixture _classFixture;

    public ValidateNewComment(CommentServiceClassFixture classFixture)
    {
        ArgumentNullException.ThrowIfNull(classFixture);

        _classFixture = classFixture;
    }

    [Theory]
    [InlineData(1, "")]
    public async Task GivenEmptyText_ShouldReturnInvalid(long id, string text)
    {
        await SetUpAsync();

        var commentServiceContext = new CommentServiceContext(
            new OptionsWrapper<CommentServiceContextOptions>(
                new CommentServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));

        var sut = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));

        var newCommentCommand = new NewCommentCommand
        {
            Text = text,
            TicketId = id
        };

        var validationResult = await sut.ValidateNewCommentAsync(newCommentCommand);

        validationResult.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, "comment")]
    [InlineData(2, "comment")]
    public async Task GivenNotExistingTicketId_ShouldReturnInvalid(long id, string text)
    {
        await SetUpAsync();

        var commentServiceContext = new CommentServiceContext(
            new OptionsWrapper<CommentServiceContextOptions>(
                new CommentServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));

        var sut = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));

        var newCommentCommand = new NewCommentCommand
        {
            Text = text,
            TicketId = id
        };

        var validationResult = await sut.ValidateNewCommentAsync(newCommentCommand);

        validationResult.IsValid.Should().BeFalse();
    }


    [Theory]
    [InlineData(1, "comment")]
    public async Task GivenValidNewCommentCommand_ReturnsValid(long id, string text)
    {
        await SetUpAsync();

        var commentServiceContext = new CommentServiceContext(
            new OptionsWrapper<CommentServiceContextOptions>(
                new CommentServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));

        var sut = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));

        var newCommentCommand = new NewCommentCommand
        {
            Text = text,
            TicketId = id
        };

        var validationResult = await sut.ValidateNewCommentAsync(newCommentCommand);

        validationResult.IsValid.Should().BeTrue();
    }

    private async Task SetUpAsync()
    {
        await using var context = new DefaultContext(_classFixture.ConnectionString);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());

        var existingTicket = fixture.Create<Ticket>();
        existingTicket.Id = 1;
        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();
    }
}