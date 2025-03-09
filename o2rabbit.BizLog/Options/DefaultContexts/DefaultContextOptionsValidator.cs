using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Options.ProcessServiceContext;

internal class DefaultContextOptionsValidator : AbstractValidator<DefaultContextOptions>
{
    public DefaultContextOptionsValidator()
    {
        RuleFor(o => o.ConnectionString).NotEmpty().MustConnectToDatabase();
    }
}