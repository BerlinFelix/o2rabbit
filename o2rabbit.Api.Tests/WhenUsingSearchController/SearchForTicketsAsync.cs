using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using o2rabbit.Api.Controllers.Searches;
using o2rabbit.Api.Options.Search;
using o2rabbit.BizLog.Abstractions.Services;
using SearchOptions = o2rabbit.Api.Options.Search.SearchOptions;

namespace o2rabbit.Api.Tests.WhenUsingSearchController;

public class SearchForTicketsAsync
{
    public SearchForTicketsAsync()
    {
    }

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
        fixture.Customize(new AutoMoqCustomization());
        ;
        var searchServiceMock = fixture.Freeze<Mock<ITicketService>>();

        var sut = new SearchController(new SearchOptionsValidator(), searchServiceMock.Object);

        var response = await sut.SearchForTicketsAsync(searchOptions);

        response.Result.Should().BeOfType<BadRequestResult>();
    }
}