using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Models;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketValidator;

public class ValidateUpdatedTicket : IClassFixture<TicketValidatorClassFixture>, IAsyncLifetime
{
    private readonly TicketValidatorClassFixture _classFixture;
    private readonly Fixture _fixture;
    private readonly TicketValidator _sut;
    private readonly TicketServiceContext _ticketContext;
    private readonly NewTicketValidator _newTicketValidator;
    private readonly UpdatedTicketValidator _updatedTicketValidator;

    public ValidateUpdatedTicket(TicketValidatorClassFixture classFixture)
    {
        _classFixture = classFixture;
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());
        _ticketContext = new TicketServiceContext(new OptionsWrapper<TicketServiceContextOptions>(
            new TicketServiceContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString
            }));
        _newTicketValidator = new NewTicketValidator(_ticketContext);
        _updatedTicketValidator = new UpdatedTicketValidator(_ticketContext);
        _sut = new TicketValidator(_newTicketValidator, _updatedTicketValidator);
    }

    [Fact]
    public async Task GivenExistingTicket_ReturnOk()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var context = new DefaultContext(_classFixture.ConnectionString);
        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();

        var updatedTicket = _fixture.Create<Ticket>();
        updatedTicket.Id = existingTicket.Id;
        updatedTicket.Name = "Updated";

        var update = new TicketUpdate(existingTicket, updatedTicket);

        var result = await _sut.ValidateAsync(update);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task GivenTwoDifferentTicketIds_ReturnInvalid()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var context = new DefaultContext(_classFixture.ConnectionString);
        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();

        var newTicket = _fixture.Create<Ticket>();
        newTicket.Id = existingTicket.Id + 1;

        var update = new TicketUpdate(existingTicket, newTicket);

        var result = await _sut.ValidateAsync(update);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GivenUpdateWithNotExistingProcessId_ReturnInvalid()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var context = new DefaultContext(_classFixture.ConnectionString);
        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();

        var updatedTicket = existingTicket.DeepClone();
        updatedTicket.ProcessId = 11;
        var update = new TicketUpdate(existingTicket, updatedTicket);

        var result = await _sut.ValidateAsync(update);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GivenUpdateWithChildren_ReturnsInvalid()
    {
        var existingTicket = _fixture.Create<Ticket>();
        var context = new DefaultContext(_classFixture.ConnectionString);
        context.Tickets.Add(existingTicket);
        await context.SaveChangesAsync();

        var updatedTicket = existingTicket.DeepClone();
        var child = _fixture.Create<Ticket>();
        updatedTicket.Children.Add(child);
        var update = new TicketUpdate(existingTicket, updatedTicket);

        var result = await _sut.ValidateAsync(update);

        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(e => e.PropertyName == $"{nameof(TicketUpdate.Update)}.{nameof(Ticket.Children)}");
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