using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.UpdatedTicketDto;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class UpdateAsync : IAsyncLifetime, IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly TicketService _sut;
    private readonly TicketServiceContext _ticketContext;

    public UpdateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        _fixture = new Fixture();
        _fixture.Customize(
            new CompositeCustomization(
                new TicketHasNoProcessNoParentsNoChildren(),
                new UpdatedTicketHasNoParent()
            )
        );

        _ticketContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidator = new TicketValidator(new NewTicketValidator(_ticketContext),
            new UpdatedTicketValidator(_ticketContext));
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        _sut = new TicketService(_ticketContext, loggerMock.Object, ticketValidator, searchOptionsValidatorMock.Object);
    }

    public async Task InitializeAsync()
    {
        await using var migrationContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await migrationContext.Database.EnsureCreatedAsync();

        var existingTicket = _fixture.Create<Ticket>();
        var existingTicket2 = _fixture.Create<Ticket>();
        existingTicket.Id = 1;
        existingTicket2.Id = 2;

        _defaultContext.Add(existingTicket);
        _defaultContext.Add(existingTicket2);

        await _defaultContext.SaveChangesAsync();

        _defaultContext.Entry(existingTicket).State = EntityState.Detached;
        _defaultContext.Entry(existingTicket2).State = EntityState.Detached;
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenValidatorReturnsInvalid_ReturnsFail(long id)
    {
        var updatedTicket = _fixture.Create<UpdateTicketCommand>();
        updatedTicket.Id = id;

        var result = await _sut.UpdateAsync(updatedTicket);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenValidatorReturnsInvalid_ReturnsErrors(long id)
    {
        var updatedTicket = _fixture.Create<UpdateTicketCommand>();
        updatedTicket.Id = id;

        var result = await _sut.UpdateAsync(updatedTicket);

        result.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenValidUpdatedTicket_SavesChanges(long id)
    {
        var updatedTicket = _fixture.Create<UpdateTicketCommand>();
        updatedTicket.Id = id;

        await _sut.UpdateAsync(updatedTicket);

        var ticket = await _defaultContext.Tickets.FindAsync(id);

        ticket.Should().NotBeNull();
        ticket.Should().BeEquivalentTo(updatedTicket);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenValidUpdatedTicket_ReturnsOk(long id)
    {
        var updatedTicket = _fixture.Create<UpdateTicketCommand>();
        updatedTicket.Id = id;

        var result = await _sut.UpdateAsync(updatedTicket);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenValidUpdatedTicket_ReturnsUpdatedTicketAsValue(long id)
    {
        var updatedTicket = _fixture.Create<UpdateTicketCommand>();
        updatedTicket.Id = id;

        var result = await _sut.UpdateAsync(updatedTicket);

        result.Value.Should().BeEquivalentTo(updatedTicket);
    }

    public async Task DisposeAsync()
    {
        await using var migrationContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await migrationContext.Database.EnsureDeletedAsync();

        await _defaultContext.DisposeAsync();
    }
}