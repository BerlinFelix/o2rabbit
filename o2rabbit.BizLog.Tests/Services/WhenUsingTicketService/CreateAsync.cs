using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.NewTicketDtoCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class CreateAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly TicketService _sut;
    private readonly DefaultContext _ticketContext;
    private readonly TicketValidator _validator;

    public CreateAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        _fixture = new Fixture();
        _fixture.Customize(new NewTicketHasNoProcessAndNoParent());

        _ticketContext =
            new DefaultContext(
                new OptionsWrapper<DefaultContextOptions>(new DefaultContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        _validator = new TicketValidator(new NewTicketValidator(_ticketContext),
            new UpdatedTicketValidator(_ticketContext));
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        _sut = new TicketService(_ticketContext, loggerMock.Object, _validator, searchOptionsValidatorMock.Object);
    }

    public async Task SetupAsync()
    {
        var migrationContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(
            new DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString }));
        await migrationContext.Database.EnsureDeletedAsync();
        await migrationContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task GivenInvalidTicket_ReturnsFail()
    {
        await SetupAsync();
        var fixture = new Fixture();
        var newTicket = fixture.Create<NewTicketCommand>();
        newTicket.ParentId = null;
        newTicket.ProcessId = null;

        var result = await _sut.CreateAsync(newTicket);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(0);
        result.Errors.Should().Contain(e => e is ValidationNotSuccessfulError);
    }

    [Fact]
    public async Task GivenNewTicket_ReturnsOkWithTicketAsValue()
    {
        await SetupAsync();
        var existingSpace = new Space()
        {
            Id = 1,
            Title = "Default Title",
            Description = "Default Description",
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };
        var newTicket = new NewTicketCommand()
        {
            Name = "Default Ticket Name",
            ParentId = null,
            ProcessId = null,
            SpaceId = 1
        };

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        context.Spaces.Add(existingSpace);
        await context.SaveChangesAsync();

        var result = await _sut.CreateAsync(newTicket);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Should().BeEquivalentTo(newTicket);
    }

    [Fact]
    public async Task GivenNewTicket_CreatesNewTicketInDatabase()
    {
        await SetupAsync();
        var existingSpace = new Space()
        {
            Id = 1,
            Title = "Default Title",
            Description = "Default Description",
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };
        var newTicket = new NewTicketCommand()
        {
            Name = "Default Ticket Name",
            ParentId = null,
            ProcessId = null,
            SpaceId = 1
        };

        var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        context.Spaces.Add(existingSpace);
        await context.SaveChangesAsync();

        var result = await _sut.CreateAsync(newTicket);

        var saved = await context.Tickets.FindAsync(result.Value.Id);

        saved.Should().NotBeNull();
        saved.Should().BeEquivalentTo(newTicket);
    }

    [Fact]
    public async Task IfAnyExceptionIsThrownWhenAccessingDb_ReturnsUnknownError()
    {
        await SetupAsync();
        var newTicket = _fixture.Create<NewTicketCommand>();
        var contextMock = new Mock<DefaultContext>();
        contextMock.Setup(x => x.Tickets).Throws<Exception>();
        var loggerMock = new Mock<ILogger<TicketService>>();

        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        var sut = new TicketService(contextMock.Object, loggerMock.Object, _validator,
            searchOptionsValidatorMock.Object);

        var result = await sut.CreateAsync(newTicket);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(error => error is UnknownError);
    }
}