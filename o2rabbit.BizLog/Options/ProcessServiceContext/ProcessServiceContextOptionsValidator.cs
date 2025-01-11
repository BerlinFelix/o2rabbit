using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Options.ProcessServiceContext;

internal class ProcessServiceContextOptionsValidator : AbstractValidator<ProcessServiceContextOptions>
{
    public ProcessServiceContextOptionsValidator()
    {
        RuleFor(o => o.ConnectionString).NotEmpty().MustConnectToDatabase();
    }
}