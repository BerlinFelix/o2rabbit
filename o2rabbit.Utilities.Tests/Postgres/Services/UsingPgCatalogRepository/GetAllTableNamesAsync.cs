using FluentAssertions;
using Npgsql;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgCatalogRepository;

public class GetAllTableNamesAsync : IClassFixture<PgCatalogRepositoryClassFixture>
{
    private readonly PgCatalogRepository _sut;
    private readonly string _connectionString;

    public GetAllTableNamesAsync(PgCatalogRepositoryClassFixture fixture)
    {
        _sut = new PgCatalogRepository();
        _connectionString = fixture.ConnectionString;
    }

    [Fact]
    public async Task GivenNullSchemaInput_ReturnsAllSchemasAndTables()
    {
        var output = await _sut.GetAllTableNamesAsync(_connectionString);

        output.Select(tableName => tableName.ToString())
            .Intersect(PgCatalogRepositoryClassFixture.ExistingTables)
            .Should()
            .HaveCount(PgCatalogRepositoryClassFixture.ExistingTables.Count);
    }


    [Fact]
    public async Task GivenExistingSchemaInput_ReturnsAllTablesBelongingToSchema()
    {
        var output = await _sut.GetAllTableNamesAsync(_connectionString,
            PgCatalogRepositoryClassFixture.ExistingSchemas[0]);

        output.Select(tableName => tableName.ToString())
            .Intersect(PgCatalogRepositoryClassFixture.ExistingTables)
            .Should()
            .HaveCount(2);
    }

    [Fact]
    public async Task GivenExistingSchemaWithoutTables_ReturnsEmptyCollection()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand("create schema schemaWithoutTables", connection);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        var result =
            await _sut.GetAllTableNamesAsync(_connectionString, "schemaWithoutTable");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenNotExistingSchemaInput_ReturnsNoTables()
    {
        var output =
            await _sut.GetAllTableNamesAsync(_connectionString, "NotExistingSchema");

        output.Select(tableName => tableName.ToString()).Intersect(PgCatalogRepositoryClassFixture.ExistingTables)
            .Should()
            .HaveCount(0);
    }

    [Fact]
    public async Task GivenInvalidConnectionString_ThrowsException()
    {
        var connectionString = "InvalidConnectionString";

        var action = () => _sut.GetAllTableNamesAsync("testSchema", connectionString);

        await action.Should().ThrowAsync<ArgumentException>();
    }
}