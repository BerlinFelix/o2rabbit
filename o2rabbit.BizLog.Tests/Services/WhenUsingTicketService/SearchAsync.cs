using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class SearchAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public SearchAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    //TODO
    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("aaaa", -1, 1)]
    [InlineData("aaaa", 1, -1)]
    public async Task GivenInvalidSearchOptions_ReturnsFailed(string searchSting, int page, int pageSize)
    {
        var ticketContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));
        var loggerMock = new Mock<ILogger<TicketService>>();
        var validator = new TicketValidator(new NewTicketValidator(ticketContext),
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
        var ticketContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString
                }));
        var loggerMock = new Mock<ILogger<TicketService>>();
        var validator = new TicketValidator(new NewTicketValidator(ticketContext),
            new UpdatedTicketValidator(ticketContext));
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        searchOptionsValidatorMock.Setup(m => m.Validate(null, It.IsAny<SearchOptions>()))
            .Returns(ValidateOptionsResult.Success);
        var sut = new TicketService(ticketContext, loggerMock.Object, validator, searchOptionsValidatorMock.Object);
        await SetUpAsync();

        var searchOptions = new SearchOptions()
        {
            SearchText = "someText",
            Page = 1,
            PageSize = 10
        };
        var result = await sut.SearchAsync(searchOptions);

        result.IsSuccess.Should().BeTrue();

        await TearDownAsync();
    }

    private async Task SetUpAsync()
    {
        await using var initializationContext = new DefaultContext(_classFixture.ConnectionString);
        await initializationContext.Database.EnsureCreatedAsync();

        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());
        var existingTickets = fixture.CreateMany<Ticket>(10).ToArray();
        for (int i = 1; i <= existingTickets.Length; i++)
        {
            existingTickets[i - 1].Id = i;
            existingTickets[i - 1].Name = $"existingTicketName{i}";
        }

        initializationContext.Tickets.AddRange(existingTickets);
        await initializationContext.SaveChangesAsync();
    }

    private async Task TearDownAsync()
    {
        await using var migrationContext = new DefaultContext(_classFixture.ConnectionString);
        await migrationContext.Database.EnsureDeletedAsync();
    }
}