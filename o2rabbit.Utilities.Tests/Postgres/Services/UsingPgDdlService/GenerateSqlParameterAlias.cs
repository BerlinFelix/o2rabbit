using FluentAssertions;
using o2rabbit.Utilities.Postgres.Services;

namespace o2rabbit.Utilities.Tests.Postgres.Services.UsingPgDdlService;

public class GenerateSqlParameterAlias
{
    private readonly PgDdlService _sut;

    public GenerateSqlParameterAlias()
    {
        _sut = new PgDdlService();
    }

    [Fact]
    void NeverReturnsTheSameString()
    {
        var results = new List<string>();

        for (var i = 0; i < 1000; i++)
        {
            var result = _sut.GenerateSqlParameterAlias();
            results.Add(result);
        }
    }
}