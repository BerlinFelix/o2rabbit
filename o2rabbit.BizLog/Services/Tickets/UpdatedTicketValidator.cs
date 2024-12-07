using FluentValidation;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

public class UpdatedTicketValidator : AbstractValidator<TicketUpdate>
{
    private readonly TicketServiceContext _context;

    public UpdatedTicketValidator(TicketServiceContext context)
    {
        _context = context;
        ArgumentNullException.ThrowIfNull(context);

        RuleFor(u => u.Old).NotNull();
        RuleFor(u => u.Update).NotNull();
        RuleFor(u => u.Old).Must((update, old) => old.Id == update.Update.Id);
        RuleFor(u => u.Update.ProcessId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await context.Processes.FindAsync(id, c).ConfigureAwait(false) != null;
        });
        // Update regarding children must be done with different endpoint
        RuleFor(u => u.Update.Children).Must(children => !children.Any());
        RuleFor(u => u.Update.ProcessId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
                return true;
            var processExists = await context.Processes.FindAsync(id, c).ConfigureAwait(false) != null;
            return processExists;
        });
    }

    /// <summary>
    /// Checks if <paramref name="ticket"/> has any children in its' ticket tree with id <paramref name="id"/>.
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<bool> ContainsInHirarchyAsync(Ticket ticket, long id,
        CancellationToken cancellationToken = default)
    {
        if (ticket.Id == id)
            return true;

        var children = await _context.Tickets
            .Where(t => t.ParentId == ticket.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var child in children)
        {
            if (await ContainsInHirarchyAsync(child, id, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }
}