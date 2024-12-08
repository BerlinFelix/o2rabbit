using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;
using o2rabbit.Core.Entities;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketValidator;

public class ValidateAsync : IClassFixture<TicketServiceClassFixture>, IAsyncLifetime
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly TicketServiceContext _context;
    private readonly TicketValidator _sut;
    private readonly Fixture _fixture;

    public ValidateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());
        _context = new TicketServiceContext(new OptionsWrapper<TicketServiceContextOptions>(
            new TicketServiceContextOptions() { ConnectionString = _classFixture.ConnectionString }));

        var newTicketValidator = new NewTicketValidator(_context);
        var updatedTicketValidator = new UpdatedTicketValidator(_context);
        _sut = new TicketValidator(newTicketValidator, updatedTicketValidator);
    }

    public async Task InitializeAsync()
    {
        await using var migrationContext = new DefaultContext(_classFixture.ConnectionString);
        await migrationContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var migrationContext = new DefaultContext(_classFixture.ConnectionString);
        await migrationContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WhenChildCircle_ValidationFails()
    {
        var parent = _fixture.Create<Ticket>();
        var child = _fixture.Create<Ticket>();
        child.ParentId = parent.Id;
        _context.Tickets.Add(child);
        _context.Tickets.Add(parent);
        await _context.SaveChangesAsync();

        var update = new UpdatedTicketCommand() { Id = parent.Id, Name = parent.Name, ParentId = child.Id };

        var validationResult = await _sut.ValidateAsync(update);

        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenGrandChildCircle_ValidationFails()
    {
        var parent = _fixture.Create<Ticket>();
        var child = _fixture.Create<Ticket>();
        child.ParentId = parent.Id;
        var grandChild = _fixture.Create<Ticket>();
        grandChild.ParentId = child.Id;
        _context.Tickets.Add(grandChild);
        _context.Tickets.Add(child);
        _context.Tickets.Add(parent);
        await _context.SaveChangesAsync();

        var update = new UpdatedTicketCommand() { Id = parent.Id, Name = parent.Name, ParentId = grandChild.Id };

        var validationResult = await _sut.ValidateAsync(update);

        validationResult.IsValid.Should().BeFalse();
    }
}