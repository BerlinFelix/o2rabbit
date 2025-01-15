using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class UpdateCommentAsync
{
    [Fact]
    public async Task CallsCommentService()
    {
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(m => m.UpdateAsync(
                It.IsAny<UpdateCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new Comment()))
            .Verifiable();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        await sut.UpdateAsync(1, new UpdateCommentCommand());

        commentServiceMock.VerifyAll();
    }

    [Fact]
    public async Task WhenCommentServiceFailsValidation_ReturnsBadRequest()
    {
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(m => m.UpdateAsync(
                It.IsAny<UpdateCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()));

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        var result = await sut.UpdateAsync(1, new UpdateCommentCommand());

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task WhenCommentServiceReturnsOk_ReturnsBadRequest()
    {
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        var updatedComment = new Comment() { Id = 2 };
        commentServiceMock.Setup(m => m.UpdateAsync(
                It.IsAny<UpdateCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedComment);

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        var result = await sut.UpdateAsync(1, new UpdateCommentCommand());

        result.Result.Should().BeOfType<OkObjectResult>();
        var okresult = (OkObjectResult)result.Result;
        okresult.Value.Should().BeOfType<DefaultCommentDto>();
        okresult.Value.Should().BeEquivalentTo(updatedComment, config =>
            config.Excluding(c => c.Ticket));
    }
}