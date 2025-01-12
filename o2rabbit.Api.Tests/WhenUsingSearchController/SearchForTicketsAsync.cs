using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Searches;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Parameters.Search;
using o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Tests.WhenUsingSearchController;

public class SearchForTicketsAsync
{
    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("aaaa", -1, 1)]
    [InlineData("aaaa", 1, -1)]
    public async Task GivenInvalidSearchOptions_ReturnsBadRequest(string searchSting, int page, int pageSize)
    {
        var searchOptions = new SearchQueryParameters()
        {
            SearchText = searchSting,
            Page = page,
            PageSize = pageSize
        };
        var fixture = new Fixture();
        var ticketServiceMock = fixture.Freeze<Mock<ITicketService>>();

        var sut = new SearchController(new SearchQueryParmetersValidator(), ticketServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [InlineData("aaaa", 1, 10)]
    public async Task GivenValidSearchOptions_ReturnsOkResult(string searchSting, int page, int pageSize)
    {
        var searchOptions = new SearchQueryParameters()
        {
            SearchText = searchSting,
            Page = page,
            PageSize = pageSize
        };

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var ticketServiceMock = new Mock<ITicketService>();
        ticketServiceMock.Setup(m =>
                m.SearchAsync(It.IsAny<SearchOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<Ticket>().ToList());

        var sut = new SearchController(new SearchQueryParmetersValidator(), ticketServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GivenTicketServiceReturnsTickets_ReturnsOkResultWithTickets()
    {
        var searchOptions = new SearchQueryParameters()
        {
            SearchText = "validSearch",
            Page = 1,
            PageSize = 10
        };

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var ticketServiceMock = new Mock<ITicketService>();
        var foundTickets = fixture.CreateMany<Ticket>().ToList();
        var expected = foundTickets.Select(t => t.ToDefaultDto());
        ticketServiceMock.Setup(m =>
                m.SearchAsync(It.IsAny<SearchOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(foundTickets);

        var sut = new SearchController(new SearchQueryParmetersValidator(), ticketServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<OkObjectResult>();
        var okObjectResult = response.Result as OkObjectResult;
        okObjectResult!.Value.Should().BeEquivalentTo(expected);
    }
}