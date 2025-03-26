using AutoFixture;
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
using o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class GetByIdAsync : IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;

    public GetByIdAsync(TicketServiceClassFixture classFixture)
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

    private async Task SetUpAsync()
    {
        await using var context = new DefaultContext(new OptionsWrapper<DefaultContextOptions>(new
            DefaultContextOptions() { ConnectionString = _classFixture.ConnectionString! }));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.AddAndSaveDefaultEntitiesAsync();

        var existingTicket = new Ticket()
        {
            ProcessId = 1,
            SpaceId = 1,
            Name = "e1"
        };
        var existingTicket2 = new Ticket()
        {
            ProcessId = 1,
            SpaceId = 1,
            Name = "e2"
        };

        context.Add(existingTicket);
        context.Add(existingTicket2);

        await context.SaveChangesAsync();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccess(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var result = await sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsIsSuccessWithTicket(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var result = await sut.GetByIdAsync(id);

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
        var sut = CreateDefaultSut();
        var ticketWithParent = new Ticket()
        {
            Name = "child",
            ProcessId = 1,
            SpaceId = 1,
            ParentId = id
        };
        ticketWithParent.ParentId = id;

        await using var context = CreateDefaultContext();
        context.Add(ticketWithParent);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(3, new GetTicketByIdOptions() { IncludeParent = true });

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
        var sut = CreateDefaultSut();
        var ticketWithParent = new Ticket()
        {
            Name = "child",
            ProcessId = 1,
            SpaceId = 1,
            ParentId = id
        };
        ticketWithParent.ParentId = id;

        await using var context = CreateDefaultContext();
        context.Add(ticketWithParent);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(id, new GetTicketByIdOptions() { IncludeChildren = true });

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
        var sut = CreateDefaultSut();
        var ticketWithParent = new Ticket()
        {
            Name = "child",
            ProcessId = 1,
            SpaceId = 1,
            ParentId = id
        };
        ticketWithParent.ParentId = id;

        await using var context = CreateDefaultContext();
        context.Add(ticketWithParent);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(id);

        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Ticket>();
        result.Value.Id.Should().Be(id);
        result.Value.Children.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task GivenNotExistingId_ReturnsInvalidIdError(long id)
    {
        await SetUpAsync();
        var sut = CreateDefaultSut();
        var result = await sut.GetByIdAsync(id);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e is InvalidIdError);
    }

    [Fact]
    public async Task GivenComments_ReturnsCommentsWhenRequested()
    {
        await SetUpAsync();
        var fixture = new Fixture();
        fixture.Customize(
            new TicketHasNoProcessNoParentsNoChildren());
        var DefaultContext =
            CreateDefaultSut();

        var sut = CreateDefaultSut();

        var comment = new TicketComment()
        {
            Text = "Comment 1",
            TicketId = 1,
            Ticket = null,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };
        var context = CreateDefaultContext();
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

        var sut = CreateDefaultSut();

        var comment = new TicketComment()
        {
            Text = "Comment 1",
            TicketId = 1,
            Ticket = null,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };
        var context = CreateDefaultContext();
        context.Add(comment);
        await context.SaveChangesAsync();

        var result = await sut.GetByIdAsync(1, new GetTicketByIdOptions() { IncludeComments = false });

        result.IsSuccess.Should().BeTrue();
        result.Value.Comments.Should().BeEmpty();
    }
}