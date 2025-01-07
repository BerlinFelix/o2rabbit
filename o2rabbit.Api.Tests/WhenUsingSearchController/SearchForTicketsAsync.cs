using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Searches;
using o2rabbit.Api.Options.Search;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using SearchOptions = o2rabbit.Api.Options.Search.SearchOptions;

namespace o2rabbit.Api.Tests.WhenUsingSearchController;

public class SearchForTicketsAsync
{
    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("aaaa", -1, 1)]
    [InlineData("aaaa", 1, -1)]
    public async Task GivenInvalidSearchOptions_ReturnsBadRequest(string searchSting, int page, int pageSize)
    {
        var searchOptions = new SearchOptions()
        {
            SearchText = searchSting,
            Page = page,
            PageSize = pageSize
        };
        var fixture = new Fixture();
        var ticketServiceMock = fixture.Freeze<Mock<ITicketService>>();

        var sut = new SearchController(new SearchOptionsValidator(), ticketServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [InlineData("aaaa", 1, 10)]
    public async Task GivenValidSearchOptions_ReturnsOkResult(string searchSting, int page, int pageSize)
    {
        var searchOptions = new SearchOptions()
        {
            SearchText = searchSting,
            Page = page,
            PageSize = pageSize
        };

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var ticketServiceMock = new Mock<ITicketService>();
        ticketServiceMock.Setup(m =>
                m.SearchAsync(It.IsAny<BizLog.Abstractions.Options.SearchOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<Ticket>().ToList());

        var sut = new SearchController(new SearchOptionsValidator(), ticketServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<OkObjectResult>();
    }
}