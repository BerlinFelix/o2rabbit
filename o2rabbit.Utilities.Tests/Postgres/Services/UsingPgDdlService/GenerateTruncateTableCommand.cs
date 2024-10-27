using FluentAssertions;
using Npgsql;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class GenerateTruncateTableCommand : IClassFixture<PgDdlServiceClassFixture>
{
    private readonly PgDdlService _sut;

    public GenerateTruncateTableCommand()
    {
        _sut = new PgDdlService();
    }

    [Fact]
    async Task GeneratesCommandThatTruncatesTable()
    {
        await using var connection = new NpgsqlConnection(PgDdlServiceClassFixture.ConnectionString);
        await connection.OpenAsync();
        var getCountCommand = new NpgsqlCommand($"select count(*) from {PgDdlServiceClassFixture.TableName}", connection);

        var previousCount = (long?)await getCountCommand.ExecuteScalarAsync();
        previousCount.Should().NotBeNull();
        previousCount.Should().BeGreaterThan(0);

        await using var command = _sut.GenerateTruncateTableCommand("public", "testTable", connection);
        await command.ExecuteNonQueryAsync();
        
        var currentCount = await getCountCommand.ExecuteScalarAsync();
        
        currentCount.Should().NotBeNull();
        currentCount.Should().Be(0);
    }
}