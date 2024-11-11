using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class CreateAsync : IClassFixture<TicketServiceClassFixture>, IAsyncLifetime
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly Fixture _fixture;
    private readonly TicketServiceContext _context;
    private readonly Mock<ILogger<TicketService>> _loggerMock;
    private readonly TicketService _sut;

    public CreateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;

        _fixture = new Fixture();
        _fixture.Customize(
            new TicketHasNoProcessNoParentsNoChildren());
        _context = new TicketServiceContext(
            new OptionsWrapper<TicketServiceContextOptions>(
                new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString ??
                                       throw new TypeInitializationException(nameof(_classFixture), null)
                }));

        _loggerMock = new Mock<ILogger<TicketService>>();

        _sut = new TicketService(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task GivenNullInput_ReturnsError()
    {
        // Arrange
        Ticket? ticket = null;

        // Act
        var result = await _sut.CreateAsync(ticket!);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenInputWithExistingId_ReturnsInvalidIdError()
    {
        var existing = _fixture.Create<Ticket>();
        await _context.AddAsync(existing);
        await _context.SaveChangesAsync();

        var ticket = _fixture.Create<Ticket>();
        ticket.Id = existing.Id;

        var result = await _sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is InvalidIdError);
    }

    [Fact]
    public async Task GivenNewTicket_ReturnsOk()
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

        var script = context.Tickets.Where(t => t.Id == result.Value.Id).ToQueryString();
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

        var sut = new TicketService(contextMock.Object, loggerMock.Object);

        var result = await sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
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
}