using FluentValidation;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

public class NewTicketValidator : AbstractValidator<Ticket>
{
    private readonly TicketServiceContext _context;

    public NewTicketValidator(TicketServiceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        RuleFor(t => t).NotNull();
        RuleFor(t => t.Id).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Id).MustAsync(async (id, c) =>
        {
            var exitingTicket = await
                _context.Tickets.FindAsync(id, c).ConfigureAwait(false);
            return exitingTicket == null;
        });
        RuleFor(t => t.ProcessId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await _context.Processes.FindAsync(id).ConfigureAwait(false) != null;
        });
    }
}