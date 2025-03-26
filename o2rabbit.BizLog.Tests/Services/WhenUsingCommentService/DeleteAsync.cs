using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingCommentService;

public class DeleteAsync : IClassFixture<CommentServiceClassFixture>
{
    private readonly CommentServiceClassFixture _classFixture;

    public DeleteAsync(CommentServiceClassFixture classFixture)
    {
        ArgumentNullException.ThrowIfNull(classFixture);

        _classFixture = classFixture;
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenSuccess_ReturnsComment(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateDefaultContext();

        var result = await sut.DeleteAsync(id);

        var comment = await context.TicketComments.FindAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenCommentIsAlreadyDeleted_ReturnsFalseAndComment()
    {
        await SetUpAsync();

        var deletedComment = new TicketComment()
        {
            Created = DateTimeOffset.UtcNow,
            DeletedAt = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow,
            Text = "comment",
            TicketId = 1,
        };
        var context = CreateDefaultContext();

        context.Add(deletedComment);
        await context.SaveChangesAsync();
        var sut = CreateDefaultSut();

        var result = await sut.DeleteAsync(deletedComment.Id);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e is AlreadyDeletedError);
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenCalled_SetsTextEmpty(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateDefaultContext();

        var result = await sut.DeleteAsync(id);

        var comment = await context.TicketComments.FindAsync(id);

        comment.Should().NotBeNull();
        comment.Text.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenCalled_SetDeletedDateAccordingly(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateDefaultContext();

        await sut.DeleteAsync(id);

        var comment = await context.TicketComments.FindAsync(id);

        comment.Should().NotBeNull();
        comment.DeletedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenIdExists_DoesNotDeleteComment(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateDefaultContext();

        var result = await sut.DeleteAsync(id);

        var commentStillExists = await context.TicketComments.AnyAsync(c => c.Id == id);

        commentStillExists.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenIdExists_ReturnsSuccess(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();

        var result = await sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(-1)]
    public async Task WhenIdNotExists_ReturnsFalse(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();

        var result = await sut.DeleteAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(-1)]
    public async Task WhenIdNotExists_ReturnsInvalidIdError(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();

        var result = await sut.DeleteAsync(id);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    private async Task SetUpAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        var existingTicket = new Ticket { Name = "ticket", ProcessId = 1, SpaceId = 1 };

        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();

        var existingComment = new TicketComment
            { Text = "comment", TicketId = 1, Created = DateTimeOffset.UtcNow, LastModified = DateTimeOffset.UtcNow };

        context.Add(existingComment);
        await context.SaveChangesAsync();
    }

    private DefaultContext CreateDefaultContext()
    {
        return new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString
            }));
    }

    private CommentService CreateDefaultSut()
    {
        var commentServiceContext = CreateDefaultContext();

        var validator = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));
        var loggerMock = new Mock<ILogger<CommentService>>();

        var sut = new CommentService(commentServiceContext, loggerMock.Object, validator);
        return sut;
    }
}