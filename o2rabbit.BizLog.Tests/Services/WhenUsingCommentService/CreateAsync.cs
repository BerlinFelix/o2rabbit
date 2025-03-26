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

public class CreateAsync : IClassFixture<CommentServiceClassFixture>
{
    private readonly CommentServiceClassFixture _classFixture;

    public CreateAsync(CommentServiceClassFixture classFixture)
    {
        ArgumentNullException.ThrowIfNull(classFixture);

        _classFixture = classFixture;
    }

    [Fact]
    public async Task GivenInvalidNewCommentCommand_ReturnsFailed()
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var newCommentCommand = new NewCommentCommand()
        {
            Text = "",
            TicketId = -1
        };

        var result = await sut.CreateAsync(newCommentCommand);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e is ValidationNotSuccessfulError);
    }

    [Fact]
    public async Task GivenValidCommand_ReturnsSuccessWithComment()
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var newCommentCommand = new NewCommentCommand()
        {
            Text = "comment",
            TicketId = 1
        };

        var result = await sut.CreateAsync(newCommentCommand);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<TicketComment>();
        result.Value.TicketId.Should().Be(1);
    }

    [Fact]
    public async Task GivenValidCommand_PersistsInDb()
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var newCommentCommand = new NewCommentCommand()
        {
            Text = "comment",
            TicketId = 1
        };

        var result = await sut.CreateAsync(newCommentCommand);

        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        var comment = await context.TicketComments.FindAsync(result.Value.Id);
        comment.Should().NotBeNull();
    }

    private CommentService CreateDefaultSut()
    {
        var defaultContext = new DefaultContext(
            new OptionsWrapper<DefaultContextOptions>(
                new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));

        var validator = new CommentValidator(new NewCommentValidator(defaultContext),
            new UpdatedCommentValidator(defaultContext));
        var loggerMock = new Mock<ILogger<CommentService>>();

        var sut = new CommentService(defaultContext, loggerMock.Object, validator);
        return sut;
    }

    private async Task SetUpAsync()
    {
        // Idee: schnellere Tests indem man statt pro fixture einen Container insgesamt nur einen Container nimmt. Jede Fixture erstellt daf"ur eine Db mit eigenem Namen und dann sollte der Connectionstring stimmen.
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        var existingTicket = new Ticket { Id = 1, Name = "ticket", ProcessId = 1, SpaceId = 1 };

        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();
    }
}