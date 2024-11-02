using FluentAssertions;

namespace o2rabbit.Utilities.Tests.Extensions.RuleBuilderExtensions;

public class MustConnectToDatabase : IClassFixture<MustConnectToDatabaseClassFixture>
{
    private readonly MustConnectToDatabaseClassFixture _classFixture;
    private readonly string _connectionstring;

    public MustConnectToDatabase(MustConnectToDatabaseClassFixture classFixture)
    {
        _classFixture = classFixture;
        _connectionstring = _classFixture.ConnectionString;
    }

    [Fact]
    public void GivenValidConnectionString_WhenExecuteAsync_ThenReturnTrue()
    {
        var options = new TestOptions()
        {
            ConnectionString = _connectionstring
        };

        var sut = new TestOptionsValidator();

        var result = sut.Validate(options);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GivenInvalidConnectionString_WhenExecuteAsync_ThenReturnFalse()
    {
        var options = new TestOptions()
        {
            ConnectionString = _connectionstring.Replace("Processes", "NotExistingDatabase")
        };

        var sut = new TestOptionsValidator();

        var result = sut.Validate(options);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GivenConnectionStringThatIsNotProperlyFormated_WhenExecuteAsync_ThenReturnFalse()
    {
        var options = new TestOptions()
        {
            ConnectionString = "invalid"
        };

        var sut = new TestOptionsValidator();

        var result = sut.Validate(options);
        result.IsValid.Should().BeFalse();
    }
}