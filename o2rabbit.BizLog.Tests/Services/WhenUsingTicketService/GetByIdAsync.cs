using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class GetByIdAsync : IAsyncLifetime, IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly TicketService _sut;
    private readonly Mock<ITicketValidator> _ticketValidatorMock;

    public GetByIdAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(_classFixture.ConnectionString);
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());

        var ticketServiceContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        _ticketValidatorMock = new Mock<ITicketValidator>();
        _sut = new TicketService(ticketServiceContext, loggerMock.Object, _ticketValidatorMock.Object);
    }

    public async Task InitializeAsync()
    {
        await using var context = new DefaultContext(_classFixture.ConnectionString);
        await context.Database.EnsureCreatedAsync();

        var existingTicket = _fixture.Create<Ticket>();
        var existingTicket2 = _fixture.Create<Ticket>();
        existingTicket.Id = 1;
        existingTicket2.Id = 2;

        _defaultContext.Add(existingTicket);
        _defaultContext.Add(existingTicket2);

        await _defaultContext.SaveChangesAsync();


        _defaultContext.Entry(existingTicket).State = EntityState.Detached;
        _defaultContext.Entry(existingTicket2).State = EntityState.Detached;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccess(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccessWithTicket(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdAndIncludeChildren_ReturnsIsSuccessWithChildren(long id)
    {
        var ticketWithParent = _fixture.Create<Ticket>();
        ticketWithParent.Id = 3;
        ticketWithParent.ParentId = id;

        _defaultContext.Add(ticketWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id, new GetTicketByIdOptions() { IncludeChildren = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Children.Should().Contain(p => p.Id == 3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdWithChildren_ReturnsTicketWithoutChildren(long id)
    {
        var ticketWithParent = _fixture.Create<Ticket>();
        ticketWithParent.Id = 3;
        ticketWithParent.ParentId = id;

        _defaultContext.Add(ticketWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Id.Should().Be(id);
        result.Value.Children.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdWithChildrenAndOptionsIncludingChildren_ReturnsTicketWithChildren(long id)
    {
        var ticketWithParent = _fixture.Create<Ticket>();
        ticketWithParent.ParentId = id;

        _defaultContext.Add(ticketWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id, new GetTicketByIdOptions() { IncludeChildren = true });

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Ticket>();
        // Note that you cannot use ...Should().BeEquivalentTo(), weil Parent-Child eine zirkulaere Referenz enthaelt.
        result.Value.Id.Should().Be(id);
        result.Value.Children.Should().HaveCount(1);
        result.Value.Children.First().ParentId.Should().Be(id);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsIsFail(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsInvalidIdError(long id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.Errors.Should().Contain(e => e is InvalidIdError);
    }

    public async Task DisposeAsync()
    {
        await using var context = new DefaultContext(_classFixture.ConnectionString);
        await context.Database.EnsureDeletedAsync();
        await _defaultContext.DisposeAsync();
    }
}