using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.CommentServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
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
        var context = CreateCommentServiceContext();

        var result = await sut.DeleteAsync(id);

        var comment = await context.Comments.FindAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenCalled_SetsTextEmpty(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateCommentServiceContext();

        var result = await sut.DeleteAsync(id);

        var comment = await context.Comments.FindAsync(id);

        comment.Should().NotBeNull();
        comment.Text.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenCalled_SetDeletedDateAccordingly(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateCommentServiceContext();

        await sut.DeleteAsync(id);

        var comment = await context.Comments.FindAsync(id);

        comment.Should().NotBeNull();
        comment.DeletedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1)]
    public async Task WhenIdExists_DoesNotDeleteComment(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var context = CreateCommentServiceContext();

        var result = await sut.DeleteAsync(id);

        var commentStillExists = await context.Comments.AnyAsync(c => c.Id == id);

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
        await using var context = CreateCommentServiceContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());

        var existingTicket = fixture.Create<Ticket>();
        context.Add(existingTicket);
        await context.SaveChangesAsync();

        var existingComment = fixture.Create<Comment>();
        existingComment.Id = 1;
        existingComment.Created = DateTime.UtcNow;
        existingComment.LastModified = DateTime.UtcNow;
        existingComment.DeletedAt = null;

        context.Add(existingComment);
        await context.SaveChangesAsync();
    }

    private CommentServiceContext CreateCommentServiceContext()
    {
        return new CommentServiceContext(new OptionsWrapper<CommentServiceContextOptions>(
            new CommentServiceContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString
            }));
    }

    private CommentService CreateDefaultSut()
    {
        var commentServiceContext = CreateCommentServiceContext();

        var validator = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));
        var loggerMock = new Mock<ILogger<CommentService>>();

        var sut = new CommentService(commentServiceContext, loggerMock.Object, validator);
        return sut;
    }
}