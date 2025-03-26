using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class UpdateAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public UpdateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private TicketService CreateDefaultSut()
    {
        var ticketContext = CreateDefaultContext();

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
        ticketValidatorMock.Setup(m => m.ValidateAsync(It.IsAny<UpdateTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        var sut = new TicketService(ticketContext, loggerMock.Object, ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);
        return sut;
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

    private async Task SetupAsync()
    {
        await using var context = CreateDefaultContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        var existingTicket = new Ticket()
        {
            Name = "existing Ticket 1",
            ProcessId = 1,
            SpaceId = 1,
        };
        var existingTicket2 = new Ticket()
        {
            Name = "existing Ticket 2",
            ProcessId = 1,
            SpaceId = 1,
        };

        context.AddRange(existingTicket, existingTicket2);

        await context.SaveChangesAsync();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenInvalidId_ReturnsValidationNotSuccessfulError(long id)
    {
        await SetupAsync();

        var ticketContext = CreateDefaultContext();
        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
        ticketValidatorMock.Setup(m => m.ValidateAsync(It.IsAny<UpdateTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult() { Errors = [new ValidationFailure("Id", "Invalid Id")] });
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        var sut = new TicketService(ticketContext, loggerMock.Object, ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);

        var update = new UpdateTicketCommand()
        {
            Id = id,
            Name = "Updated Ticket Name",
            ParentId = null
        };

        var result = await sut.UpdateAsync(update);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ValidationNotSuccessfulError);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenValidUpdatedTicket_SavesChanges(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateTicketCommand()
        {
            Id = id,
            Name = "Updated Ticket Name",
            ParentId = null
        };

        var result = await sut.UpdateAsync(update);

        var context = CreateDefaultContext();
        var ticket = await context.Tickets.FindAsync(id);

        ticket.Should().NotBeNull();
        ticket.Should().BeEquivalentTo(update);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenValidUpdatedTicket_ReturnsUpdatedTicketAsValue(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var update = new UpdateTicketCommand()
        {
            Id = id,
            Name = "Updated Ticket Name",
            ParentId = null
        };

        var result = await sut.UpdateAsync(update);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(update);
    }
}