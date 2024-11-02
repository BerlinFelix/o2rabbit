using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.Utilities.Tests.Extensions.RuleBuilderExtensions;

public class TestOptionsValidator : AbstractValidator<TestOptions>
{
    public TestOptionsValidator()
    {
        RuleFor(o => o.ConnectionString).NotEmpty().MustConnectToDatabase();
    }
}