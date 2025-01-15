using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.Api.Tests.AutoFixtureCustomization;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class DeleteCommentAsync
{
    [Fact]
    public async Task WhenCalled_CallsCommentService()
    {
        var fixture = new AutoMoqFixture();
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(m => m.DeleteAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok())
            .Verifiable();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        await sut.DeleteCommentAsync(1);

        commentServiceMock.VerifyAll();
    }

    [Fact]
    public async Task WhenOk_ReturnsOk()
    {
        var fixture = new AutoMoqFixture();
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(m => m.DeleteAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok())
            .Verifiable();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        var response = await sut.DeleteCommentAsync(1);

        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task WhenInvalidId_ReturnsBadRequest()
    {
        var fixture = new AutoMoqFixture();
        var ticketServiceMock = new Mock<ITicketService>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(m => m.DeleteAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new InvalidIdError()))
            .Verifiable();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);

        var response = await sut.DeleteCommentAsync(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }
}