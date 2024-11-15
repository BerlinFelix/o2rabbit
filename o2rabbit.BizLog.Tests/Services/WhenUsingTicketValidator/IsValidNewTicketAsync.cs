using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketValidator;

public class IsValidNewTicketAsync : IClassFixture<TicketValidatorClassFixture>, IAsyncLifetime
{
    private readonly TicketValidatorClassFixture _classFixture;
    private readonly Fixture _fixture;
    private readonly TicketValidator _sut;
    private readonly TicketServiceContext _ticketContext;

    public IsValidNewTicketAsync(TicketValidatorClassFixture classFixture)
    {
        _classFixture = classFixture;
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());
        _ticketContext = new TicketServiceContext(new OptionsWrapper<TicketServiceContextOptions>(
            new TicketServiceContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString
            }));
        _sut = new TicketValidator(_ticketContext);
    }

    [Fact]
    public async Task GivenNullInput_ReturnsNullInputError()
    {
        var result = await _sut.IsValidNewTicketAsync(null!);

        result.Errors.Should().ContainSingle(e => e is NullInputError);
    }


    [Fact]
    public async Task GivenTicketWithExistingId_ReturnsInvalidIdError()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var _context = new DefaultContext(_classFixture.ConnectionString);
        _context.Tickets.Add(existingTicket);
        await _context.SaveChangesAsync();

        var newTicket = _fixture.Create<Ticket>();
        newTicket.Id = existingTicket.Id;

        var result = await _sut.IsValidNewTicketAsync(newTicket);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenTicketWithNotExistingProcessId_ReturnsInvalidIdError()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoParentsAndNoChildren());
        var ticket = fixture.Create<Ticket>();
        ticket.ProcessId = 1;

        var result = await _sut.IsValidNewTicketAsync(ticket);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenValidTicket_ReturnsOk()
    {
        var ticket = _fixture.Create<Ticket>();

        var result = await _sut.IsValidNewTicketAsync(ticket);

        result.IsSuccess.Should().BeTrue();
    }

    public async Task InitializeAsync()
    {
        await using var defaultContext = new DefaultContext(_classFixture.ConnectionString);
        await defaultContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var defaultContext = new DefaultContext(_classFixture.ConnectionString);
        await defaultContext.Database.EnsureDeletedAsync();
    }
}