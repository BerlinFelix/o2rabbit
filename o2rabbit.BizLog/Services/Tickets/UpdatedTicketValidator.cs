using FluentValidation;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Models;

namespace o2rabbit.BizLog.Services.Tickets;

public class UpdatedTicketValidator : AbstractValidator<TicketUpdate>
{
    public UpdatedTicketValidator(TicketServiceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        RuleFor(u => u.Old).NotNull();
        RuleFor(u => u.New).NotNull();
        RuleFor(u => u.Old).Must((update, old) => old.Id == update.New.Id);
        RuleFor(u => u.New.ProcessId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await context.Processes.FindAsync(id, c).ConfigureAwait(false) != null;
        });
    }
}