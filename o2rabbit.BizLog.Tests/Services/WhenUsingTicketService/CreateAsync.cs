using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class CreateAsync : IClassFixture<TicketServiceClassFixture>, IAsyncLifetime
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly TicketService _sut;
    private readonly TicketServiceContext _ticketContext;
    private readonly TicketValidator _ticketValidator;

    public CreateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(_classFixture.ConnectionString);
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());

        _ticketContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        // Note that FluentValidation strongly suggests not to mock its validators.
        // Instead one should actual validation
        _ticketValidator = new TicketValidator(new NewTicketValidator(_ticketContext),
            new UpdatedTicketValidator(_ticketContext));
        _sut = new TicketService(_ticketContext, loggerMock.Object, _ticketValidator);
    }

    public async Task InitializeAsync()
    {
        var migrationContext = new DefaultContext(_classFixture.ConnectionString);
        await migrationContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        var migrationContext = new DefaultContext(_classFixture.ConnectionString);
        await migrationContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GivenValidatorReturnsInvalid_ReturnsFail()
    {
        var existing = _fixture.Create<Ticket>();
        await _ticketContext.AddAsync(existing);
        await _ticketContext.SaveChangesAsync();

        var ticket = _fixture.Create<Ticket>();
        ticket.Id = existing.Id;

        var result = await _sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidatorReturnsInvalid_ReturnsValidationError()
    {
        var existing = _fixture.Create<Ticket>();
        await _ticketContext.AddAsync(existing);
        await _ticketContext.SaveChangesAsync();

        var ticket = _fixture.Create<Ticket>();
        ticket.Id = existing.Id;

        var result = await _sut.CreateAsync(ticket);

        result.Errors.Should().Contain(e => e is ValidationNotSuccessfulError);
    }

    [Fact]
    public async Task GivenNewValidTicket_ReturnsOk()
    {
        var ticket = _fixture.Create<Ticket>();

        var result = await _sut.CreateAsync(ticket);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GivenNewTicket_ReturnsOkWithTicketAsValue()
    {
        var ticket = _fixture.Create<Ticket>();

        var result = await _sut.CreateAsync(ticket);

        result.Value.Should().BeOfType<Ticket>();
        result.Value.Should().Be(ticket);
    }

    [Fact]
    public async Task GivenNewTicket_CreatesNewTicketInDatabase()
    {
        var ticket = _fixture.Create<Ticket>();
        ticket.Id = 0;
        var result = await _sut.CreateAsync(ticket);

        var context = new DefaultContext(_classFixture.ConnectionString);

        var saved = await context.Tickets.FindAsync(result.Value.Id);

        saved.Should().NotBeNull();
        saved.Name.Should().Be(ticket.Name);
    }

    [Fact]
    public async Task IfAnyExceptionIsThrownWhenAccessingDb_ReturnsUnknownError()
    {
        var ticket = _fixture.Create<Ticket>();
        var contextMock = new Mock<TicketServiceContext>();
        contextMock.Setup(x => x.Tickets).Throws<Exception>();
        var loggerMock = new Mock<ILogger<TicketService>>();

        var sut = new TicketService(contextMock.Object, loggerMock.Object, _ticketValidator);

        var result = await sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
    }
}