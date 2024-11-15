using AutoFixture;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.BizLog.Tests.Errors;
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
    private readonly Mock<ITicketValidator> _ticketValidatorMock;

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

        _ticketValidatorMock = new Mock<ITicketValidator>();
        _sut = new TicketService(_context, _loggerMock.Object, _ticketValidatorMock.Object);
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
    public async Task GivenValidatorReturnsAnyError_ReturnsTheError()
    {
        _ticketValidatorMock.Setup(m => m.IsValidNewTicketAsync(
                It.IsAny<Ticket>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new TestError()));
        var existing = _fixture.Create<Ticket>();
        await _context.AddAsync(existing);
        await _context.SaveChangesAsync();

        var ticket = _fixture.Create<Ticket>();
        ticket.Id = existing.Id;

        var result = await _sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is TestError);
    }

    [Fact]
    public async Task GivenNewValidTicket_ReturnsOk()
    {
        _ticketValidatorMock.Setup(m => m.IsValidNewTicketAsync(
                It.IsAny<Ticket>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        var ticket = _fixture.Create<Ticket>();

        var result = await _sut.CreateAsync(ticket);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GivenNewTicket_ReturnsOkWithTicketAsValue()
    {
        _ticketValidatorMock.Setup(m => m.IsValidNewTicketAsync(
                It.IsAny<Ticket>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
        var ticket = _fixture.Create<Ticket>();

        var result = await _sut.CreateAsync(ticket);

        result.Value.Should().BeOfType<Ticket>();
        result.Value.Should().Be(ticket);
    }

    [Fact]
    public async Task GivenNewTicket_CreatesNewTicketInDatabase()
    {
        _ticketValidatorMock.Setup(m => m.IsValidNewTicketAsync(
                It.IsAny<Ticket>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
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

        var sut = new TicketService(contextMock.Object, loggerMock.Object, _ticketValidatorMock.Object);

        var result = await sut.CreateAsync(ticket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
    }
}