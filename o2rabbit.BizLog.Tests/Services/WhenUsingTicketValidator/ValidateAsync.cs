using FluentAssertions;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketValidator;

public class ValidateAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public ValidateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private TicketValidator CreateDefaultSut()
    {
        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        var newTicketValidator = new NewTicketValidator(context);
        var updatedTicketValidator = new UpdatedTicketValidator(context);
        var sut = new TicketValidator(newTicketValidator, updatedTicketValidator);
        return sut;
    }

    public async Task SetupAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();
    }

    [Fact]
    public async Task WhenChildCircle_ValidationFails()
    {
        await SetupAsync();
        var sut = CreateDefaultSut();

        var parent = new Ticket() { Name = "parent", ProcessId = 1, SpaceId = 1 };
        var child = new Ticket() { Name = "child", ProcessId = 1, SpaceId = 1, ParentId = 1 };

        await using var context =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        context.Tickets.AddRange(parent, child);

        await context.SaveChangesAsync();

        var update = new UpdateTicketCommand() { Id = parent.Id, Name = parent.Name, ParentId = child.Id };

        var validationResult = await sut.ValidateAsync(update);

        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenGrandChildCircle_ValidationFails()
    {
        await SetupAsync();
        var sut = CreateDefaultSut();

        var parent = new Ticket() { Name = "parent", ProcessId = 1, SpaceId = 1 };
        var child = new Ticket() { Name = "child", ProcessId = 1, SpaceId = 1, ParentId = 1 };
        var grandChild = new Ticket() { Name = "grandChild", ProcessId = 1, SpaceId = 1, ParentId = 2 };

        await using var context =
            new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                { ConnectionString = _classFixture.ConnectionString }));
        context.Tickets.AddRange(parent, child, grandChild);

        await context.SaveChangesAsync();

        var update = new UpdateTicketCommand() { Id = parent.Id, Name = parent.Name, ParentId = grandChild.Id };

        var validationResult = await sut.ValidateAsync(update);

        validationResult.IsValid.Should().BeFalse();
    }
}