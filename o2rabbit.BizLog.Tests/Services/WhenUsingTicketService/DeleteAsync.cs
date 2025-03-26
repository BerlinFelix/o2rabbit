using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Extensions;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class DeleteAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public DeleteAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
    }

    private TicketService CreateDefaultSut()
    {
        var ticketContext = CreateDefaultContext();

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
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
    public async Task GivenNotExistingId_ReturnsFail(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var result = await sut.DeleteAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsUnknownInvalidIdError(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var result = await sut.DeleteAsync(id);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsOk(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var result = await sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_DeletesTicket(long id)
    {
        await SetupAsync();
        var sut = CreateDefaultSut();
        var result = await sut.DeleteAsync(id);

        await using var context = CreateDefaultContext();
        var ticket = await context.Tickets.FindAsync(id);
        ticket.Should().BeNull();
    }
}