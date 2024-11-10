using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Tests.AutoFixtureCustomization;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Migrations.Context;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.BizLog.Tests.Services.WhenUsingTicketService;

public class DeleteAsync : IAsyncLifetime, IClassFixture<TicketServiceClassFixture>
{
    private readonly TicketServiceClassFixture _classFixture;
    private readonly DefaultContext _defaultContext;
    private readonly Fixture _fixture;
    private readonly PgDdlService _pgDllService;
    private readonly PgCatalogRepository _pgCatalogRepository;
    private readonly TicketService _sut;
    private readonly TicketServiceContext _ticketContext;

    public DeleteAsync(TicketServiceClassFixture classFixture)
    {
        _classFixture = classFixture;
        _defaultContext = new DefaultContext(_classFixture.ConnectionString);
        _fixture = new Fixture();
        _fixture.Customize(new TicketHasNoParentsAndNoChildren());
        _pgDllService = new PgDdlService();
        _pgCatalogRepository = new PgCatalogRepository();

        _ticketContext =
            new TicketServiceContext(
                new OptionsWrapper<TicketServiceContextOptions>(new TicketServiceContextOptions()
                {
                    ConnectionString = _classFixture.ConnectionString!
                }));

        var loggerMock = new Mock<ILogger<TicketService>>();
        _sut = new TicketService(_ticketContext, loggerMock.Object);
    }

    public async Task InitializeAsync()
    {
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
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsFail(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task GivenNotExistingId_ReturnsUnknownInvalidIdError(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.Errors.Should().ContainSingle(e => e is InvalidIdError);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_ReturnsOk(long id)
    {
        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GivenExistingId_DeletesTicket(long id)
    {
        var result = await _sut.DeleteAsync(id);

        var ticket = await _defaultContext.Tickets.FindAsync(id);
        ticket.Should().BeNull();
    }

    public async Task DisposeAsync()
    {
        var existingTables =
            await _pgCatalogRepository.GetAllTableNamesAsync(_classFixture.ConnectionString!);

        await using var connection = new NpgsqlConnection(_classFixture.ConnectionString);
        await connection.OpenAsync();
        foreach (var qualifiedTableName in existingTables)
        {
            await using var truncateStatement =
                _pgDllService.GenerateTruncateTableCommand(qualifiedTableName, connection);
            await truncateStatement.ExecuteNonQueryAsync();
        }

        await _defaultContext.DisposeAsync();
    }
}