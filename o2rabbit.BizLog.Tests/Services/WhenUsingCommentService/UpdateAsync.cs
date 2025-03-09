using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.CommentServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.BizLog.Tests.FakeFactories;
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

        var validationResult = await sut.UpdateAsync(update);

        validationResult.IsFailed.Should().BeTrue();
        validationResult.Errors.Should().ContainSingle(e => e is ValidationNotSuccessfulError);
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

        await using var context = new DefaultContext(_classFixture.ConnectionString);
        var comment = await context.Comments.FindAsync(id);
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
        await using var comparisonContext = new DefaultContext(_classFixture.ConnectionString);
        var oldComment =
            await comparisonContext.Comments.FindAsync(id);

        await sut.UpdateAsync(update);

        await using var context = new DefaultContext(_classFixture.ConnectionString);
        var comment = await context.Comments.FindAsync(id);
        comment.Should().NotBeNull();
        comment.LastModified.Should().BeAfter(oldComment.Created);
    }

    private async Task SetupAsync()
    {
        await using var setupContext = new DefaultContext(_classFixture.ConnectionString);
        await setupContext.Database.EnsureDeletedAsync();
        await setupContext.Database.EnsureCreatedAsync();

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());
        var ticket = fixture.Create<Ticket>();
        ticket.Id = 1;
        var existingComment = FakeCommentFactory.CreateComment();
        existingComment.Id = 1;

        setupContext.Add(existingComment);
        setupContext.Add(ticket);

        await setupContext.SaveChangesAsync();
    }

    private CommentService CreateDefaultSut()
    {
        var fixture = new Fixture();
        var context = new CommentServiceContext(new OptionsWrapper<CommentServiceContextOptions>(
            new CommentServiceContextOptions { ConnectionString = _classFixture.ConnectionString }));
        var loggerMock = new Mock<ILogger<CommentService>>();
        var commentValidator = new CommentValidator(
            new NewCommentValidator(context),
            new UpdatedCommentValidator(context));

        return new CommentService(context, loggerMock.Object, commentValidator);
    }
}