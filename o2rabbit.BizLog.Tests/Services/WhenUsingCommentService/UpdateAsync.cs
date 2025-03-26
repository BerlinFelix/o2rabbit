using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingCommentService;

public class UpdateAsync : IClassFixture<CommentServiceClassFixture>
{
    private readonly CommentServiceClassFixture _classFixture;

    public UpdateAsync(CommentServiceClassFixture classFixture)
    {
        ArgumentNullException.ThrowIfNull(classFixture);

        _classFixture = classFixture;
    }

    [Theory]
    [InlineData(-1, "text")]
    [InlineData(1, "")]
    public async Task GivenInvalidUpdate_ReturnsValidationNotSuccesfull(long id, string text)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateCommentCommand() { Id = id, Text = text };

        var result = await sut.UpdateAsync(update);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ValidationNotSuccessfulError);
    }

    [Theory]
    [InlineData(1, "text")]
    public async Task GivenValidUpdate_ReturnsOkWithComment(long id, string text)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateCommentCommand() { Id = id, Text = text };

        var validationResult = await sut.UpdateAsync(update);

        validationResult.IsSuccess.Should().BeTrue();
        validationResult.Value.Should().BeEquivalentTo(update);
    }

    [Theory]
    [InlineData(1, "text")]
    public async Task GivenValidUpdate_PersistsInDb(long id, string text)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateCommentCommand() { Id = id, Text = text };

        await sut.UpdateAsync(update);

        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString
            }));
        var comment = await context.TicketComments.FindAsync(id);
        comment.Should().NotBeNull();
        comment.Should().BeEquivalentTo(update);
    }

    [Theory]
    [InlineData(1, "text")]
    public async Task GivenValidUpdate_ChangesLastModified(long id, string text)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateCommentCommand() { Id = id, Text = text };
        await using var comparisonContext =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var oldComment =
            await comparisonContext.TicketComments.FindAsync(id);

        await sut.UpdateAsync(update);

        await using var context =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        var comment = await context.TicketComments.FindAsync(id);
        comment.Should().NotBeNull();
        comment.LastModified.Should().BeAfter(oldComment.Created);
    }

    private async Task SetupAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString })
        );
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        var ticket = new Ticket()
        {
            Name = "ticket",
            ProcessId = 1,
            SpaceId = 1
        };
        context.Add(ticket);
        await context.SaveChangesAsync();

        var existingComment = new TicketComment()
        {
            Text = "comment",
            TicketId = 1,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };

        context.Add(existingComment);

        await context.SaveChangesAsync();
    }

    private CommentService CreateDefaultSut()
    {
        var fixture = new Fixture();
        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        var loggerMock = new Mock<ILogger<CommentService>>();
        var commentValidator = new CommentValidator(
            new NewCommentValidator(context),
            new UpdatedCommentValidator(context));

        return new CommentService(context, loggerMock.Object, commentValidator);
    }
}