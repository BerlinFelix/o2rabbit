using FluentAssertions;
using Npgsql;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class GenerateTruncateTableCommand : IClassFixture<PgDdlServiceClassFixture>
{
    private readonly PgDdlService _sut;
    private readonly string _connectionString;

    public GenerateTruncateTableCommand(PgDdlServiceClassFixture fixture)
    {
        _sut = new PgDdlService();
        _connectionString = fixture.ConnectionString;
    }

    [Fact]
    async Task GeneratesCommandThatTruncatesTable()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var getCountCommand =
            new NpgsqlCommand($"select count(*) from {PgDdlServiceClassFixture.TableName}", connection);

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