using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Tickets;

public class NewTicketValidator : AbstractValidator<NewTicketDto>
{
    private readonly TicketServiceContext _context;

    public NewTicketValidator(TicketServiceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        RuleFor(t => t).NotNull();
        RuleFor(t => t.ParentId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await _context.Tickets.FindAsync(id).ConfigureAwait(false) != null;
        }).WithMessage("Parent not found");
        RuleFor(t => t.ProcessId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await _context.Processes.FindAsync(id).ConfigureAwait(false) != null;
        }).WithMessage("Process not found");
        RuleFor(t => t.Name).NotEmpty();
    }
}