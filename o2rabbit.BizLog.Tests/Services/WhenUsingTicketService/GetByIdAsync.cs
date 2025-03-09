using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Tickets;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class GetByIdAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly TicketService _sut;
    private readonly Mock<ITicketValidator> _ticketValidatorMock;

    public GetByIdAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
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
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        _sut = new TicketService(ticketServiceContext, loggerMock.Object, _ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);
    }

    public async Task SetUpAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
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
        await SetUpAsync();
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccessWithTicket(long id)
    {
        await SetUpAsync();
        var result = await _sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdAndIncludeParent_ReturnsIsSuccessWithParent(long id)
    {
        await SetUpAsync();
        var ticketWithParent = _fixture.Create<Ticket>();
        ticketWithParent.Id = 3;
        ticketWithParent.ParentId = id;

        _defaultContext.Add(ticketWithParent);
        await _defaultContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(3, new GetTicketByIdOptions() { IncludeParent = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Parent.Should().NotBeNull();
        result.Value.Parent.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingIdAndIncludeChildren_ReturnsIsSuccessWithChildren(long id)
    {
        await SetUpAsync();
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
        await SetUpAsync();
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
        await SetUpAsync();
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
        await SetUpAsync();
        var result = await _sut.GetByIdAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsInvalidIdError(long id)
    {
        await SetUpAsync();
        var result = await _sut.GetByIdAsync(id);

        result.Errors.Should().Contain(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenComments_ReturnsCommentsWhenRequested()
    {
        await SetUpAsync();
        var fixture = new Fixture();
        fixture.Customize(
            new TicketHasNoProcessNoParentsNoChildren());
        var ticketServiceContext =
            GetTicketServiceContext();

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        var sut = new TicketService(ticketServiceContext, loggerMock.Object, ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);

        var comment = fixture.Create<TicketComment>();
        comment.TicketId = 1;
        comment.Ticket = null;
        comment.Created = DateTime.UtcNow;
        comment.LastModified = DateTime.UtcNow;
        var context = GetTicketServiceContext();
        context.Add(comment);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(1, new GetTicketByIdOptions() { IncludeComments = true });

        result.IsSuccess.Should().BeTrue();
        result.Value.Comments.Should().HaveCount(1);
        result.Value.Comments.First().Text.Should().Be(comment.Text);
    }

    [Fact]
    public async Task GivenComments_ReturnsNoCommentsWhenNotRequested()
    {
        await SetUpAsync();
        var fixture = new Fixture();
        fixture.Customize(
            new TicketHasNoProcessNoParentsNoChildren());
        var ticketServiceContext =
            GetTicketServiceContext();

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        var sut = new TicketService(ticketServiceContext, loggerMock.Object, ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);

        var comment = fixture.Create<TicketComment>();
        comment.TicketId = 1;
        comment.Ticket = null;
        comment.Created = DateTime.UtcNow;
        comment.LastModified = DateTime.UtcNow;
        var context = GetTicketServiceContext();
        context.Add(comment);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(1, new GetTicketByIdOptions() { IncludeComments = false });

        result.IsSuccess.Should().BeTrue();
        result.Value.Comments.Should().BeEmpty();
    }

    private TicketServiceContext GetTicketServiceContext()
    {
        return new TicketServiceContext(
            new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
            {
                ConnectionString = _classFixture.ConnectionString!
            }));
    }

    private TicketService SetUpDefaultTicketService()
    {
        var fixture = new Fixture();
        fixture.Customize(new TicketHasNoProcessNoParentsNoChildren());

        var ticketServiceContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        var ticketValidatorMock = new Mock<ITicketValidator>();
        var searchOptionsValidatorMock = new Mock<IValidateOptions<SearchOptions>>();
        return new TicketService(ticketServiceContext, loggerMock.Object, _ticketValidatorMock.Object,
            searchOptionsValidatorMock.Object);
    }
}