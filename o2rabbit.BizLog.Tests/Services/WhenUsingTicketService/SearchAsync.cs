using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class SearchAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public SearchAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private DefaultContext CreateDefaultContext()
    {
        var ticketContext =
            new DefaultContext(
                new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));
        return ticketContext;
    }

    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("aaaa", -1, 1)]
    [InlineData("aaaa", 1, -1)]
    public async Task GivenInvalidSearchOptions_ReturnsFailed(string searchSting, int page, int pageSize)
    {
        var ticketContext =
            new DefaultContext(
                new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));
        var loggerMock = new Mock<ILogger<TicketService>>();
        var validator = new TicketValidator(new NewTicketValidator(),
            new UpdatedTicketValidator(ticketContext));
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        searchOptionsValidatorMock.Setup(m => m.Validate(null, It.IsAny<SearchOptions>()))
            .Returns(ValidateOptionsResult.Fail(string.Empty));
        var sut = new TicketService(ticketContext, loggerMock.Object, validator, searchOptionsValidatorMock.Object);
        var searchOptions = new SearchOptions()
        {
            SearchText = searchSting,
            Page = page,
            PageSize = pageSize
        };

        var result = await sut.SearchAsync(searchOptions);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenResponseFromContext_ReturnsSuccess()
    {
        await SetUpDatabaseAsync();
        var sut = SetupDefaultSut();

        var searchOptions = new SearchOptions()
        {
            SearchText = "someText",
            Page = 1,
            PageSize = 10
        };
        var result = await sut.SearchAsync(searchOptions);

        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public async Task GivenResponseContainingTicketsFromContext_ReturnsTickets()
    {
        var sut = SetupDefaultSut();
        await SetUpDatabaseAsync();

        var searchOptions = new SearchOptions()
        {
            SearchText = "existingTicketName",
            Page = 1,
            PageSize = 10
        };
        var result = await sut.SearchAsync(searchOptions);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenResponseContainingTicketsFromContext_ReturnsAtMostPageSizeTickets()
    {
        var sut = SetupDefaultSut();
        await SetUpDatabaseAsync();

        var searchOptions = new SearchOptions()
        {
            SearchText = "existingTicketName",
            Page = 1,
            PageSize = 5
        };
        var result = await sut.SearchAsync(searchOptions);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count().Should().BeLessThanOrEqualTo(searchOptions.PageSize);
    }

    [Fact]
    public async Task GivenRequestedPage_ReturnsCorrectPage()
    {
        var sut = SetupDefaultSut();
        await SetUpDatabaseAsync();

        var searchOptions = new SearchOptions()
        {
            SearchText = "existingTicketName",
            Page = 2,
            PageSize = 5
        };
        var result = await sut.SearchAsync(searchOptions);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().OnlyContain(t => t.Id <= 10 && t.Id >= 6);
    }

    private TicketService SetupDefaultSut()
    {
        var ticketContext =
            new DefaultContext(
                new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));
        var loggerMock = new Mock<ILogger<TicketService>>();
        var validator = new TicketValidator(new NewTicketValidator(),
            new UpdatedTicketValidator(ticketContext));
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        searchOptionsValidatorMock.Setup(m => m.Validate(null, It.IsAny<SearchOptions>()))
            .Returns(ValidateOptionsResult.Success);
        var sut = new TicketService(ticketContext, loggerMock.Object, validator, searchOptionsValidatorMock.Object);
        return sut;
    }

    private async Task SetUpDatabaseAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        for (int i = 1; i <= 10; i++)
        {
            context.Tickets.Add(new Ticket()
            {
                Name = $"existingTicketName{i}",
                ProcessId = 1,
                SpaceId = 1,
            });
        }

        await context.SaveChangesAsync();
    }
}