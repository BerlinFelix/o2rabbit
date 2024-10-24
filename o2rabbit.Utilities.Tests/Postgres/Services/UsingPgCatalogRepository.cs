using FluentAssertions;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.Utilities.Tests.Postgres.Services;

public class UsingPgCatalogRepository : IClassFixture<PgCatalogRepositoryClassFixture>
{
    private readonly PgCatalogRepository _sut;

    public UsingPgCatalogRepository()
    {
        _sut = new PgCatalogRepository();
    }

    [Fact]
    public async Task GivenNullSchemaInput_ReturnsAllTables()
    {
        var output = await _sut.GetTableNamesAsync(PgCatalogRepositoryClassFixture.ConnectionString);

        output.Intersect(PgCatalogRepositoryClassFixture.ExistingSchemas).Should()
            .HaveCount(PgCatalogRepositoryClassFixture.ExistingSchemas.Count);

    }

    [Fact]
    public async Task GivenInvalidConnectionString_ThrowsException()
    {
        var connectionString = "InvalidConnectionString";

        var action = () => _sut.GetTableNamesAsync("testSchema", connectionString);

        await action.Should().ThrowAsync<ArgumentException>();
    }
}