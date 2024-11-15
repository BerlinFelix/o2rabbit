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

public class IsValidUpdatedTicketAsync : IClassFixture<TicketValidatorClassFixture>, IAsyncLifetime
{
    private readonly TicketValidatorClassFixture _classFixture;
    private readonly Fixture _fixture;
    private readonly TicketValidator _sut;
    private readonly TicketServiceContext _ticketContext;

    public IsValidUpdatedTicketAsync(TicketValidatorClassFixture classFixture)
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
        var result = await _sut.IsValidUpdatedTicket(null!);

        result.Errors.Should().ContainSingle(e => e is NullInputError);
    }


    [Fact]
    public async Task GivenExistingTicket_ReturnOk()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var _context = new DefaultContext(_classFixture.ConnectionString);
        _context.Tickets.Add(existingTicket);
        await _context.SaveChangesAsync();

        var newTicket = _fixture.Create<Ticket>();
        newTicket.Id = existingTicket.Id;

        var result = await _sut.IsValidUpdatedTicket(newTicket);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GivenTicketWithNotExistingProcessId_ReturnsInvalidIdError()
    {
        var ticket = _fixture.Create<Ticket>();
        var result = await _sut.IsValidUpdatedTicket(ticket);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
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