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
using o2rabbit.BizLog.Tests.Services.WhenUsingCommentValidator;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;

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
    public async Task GivenValidCommand_ReturnsSuccessWithTicket()
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
        result.Value.Should().BeOfType<Comment>();
        result.Value.TicketId.Should().Be(1);
    }

    private CommentService CreateDefaultSut()
    {
        var commentServiceContext = new CommentServiceContext(
            new OptionsWrapper<CommentServiceContextOptions>(
                new CommentServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));

        var validator = new CommentValidator(new NewCommentValidator(commentServiceContext),
            new UpdatedCommentValidator(commentServiceContext));
        var loggerMock = new Mock<ILogger<CommentService>>();

        var sut = new CommentService(commentServiceContext, loggerMock.Object, validator);
        return sut;
    }

    private async Task SetUpAsync()
    {
        // Idee: schnellere Tests indem man statt pro fixture einen Container insgesamt nur einen Container nimmt. Jede Fixture erstellt daf"ur eine Db mit eigenem Namen und dann sollte der Connectionstring stimmen.
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