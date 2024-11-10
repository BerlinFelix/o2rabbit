using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Options.TicketServiceContext;

internal class TicketServiceContextOptionsValidator : AbstractValidator<TicketServiceContextOptions>
{
    public TicketServiceContextOptionsValidator()
    {
        RuleFor(o => o.ConnectionString).NotEmpty().MustConnectToDatabase();
    }
}