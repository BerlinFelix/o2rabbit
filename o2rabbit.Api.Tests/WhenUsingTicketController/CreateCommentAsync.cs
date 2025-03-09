using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Tickets;
using o2rabbit.Api.Models;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Tests.WhenUsingTicketController;

public class CreateCommentAsync
{
    [Fact]
    public async Task WhenCalled_CallsCommentService()
    {
        var fixture = new Fixture();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(c => c.CreateAsync(
                It.IsAny<NewCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new UnknownError()));
        var ticketServiceMock = new Mock<ITicketService>();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);
        var command = fixture.Create<NewCommentCommand>();

        await sut.CreateCommentAsync(command);

        commentServiceMock.VerifyAll();
    }

    [Fact]
    public async Task WhenCommentServiceReturnsValidationNotSuccesful_ReturnsBadRequest()
    {
        var fixture = new Fixture();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(c => c.CreateAsync(
                It.IsAny<NewCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new ValidationNotSuccessfulError()));
        var ticketServiceMock = new Mock<ITicketService>();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);
        var command = fixture.Create<NewCommentCommand>();

        var result = await sut.CreateCommentAsync(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public async Task WhenCommentServiceReturnsSuccess_ReturnsOkObjectResultWithCommentDto()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(c => c.CreateAsync(
                It.IsAny<NewCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(fixture.Create<TicketComment>()));
        var ticketServiceMock = new Mock<ITicketService>();

        var sut = new TicketController(ticketServiceMock.Object, commentServiceMock.Object);
        var command = fixture.Create<NewCommentCommand>();

        var result = await sut.CreateCommentAsync(command);

        result.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeOfType<DefaultCommentDto>();
    }
}