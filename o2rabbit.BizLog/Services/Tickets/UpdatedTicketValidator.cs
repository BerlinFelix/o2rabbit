using FluentValidation;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

public class UpdatedTicketValidator : AbstractValidator<UpdatedTicketCommand>
{
    private readonly TicketServiceContext _context;

    public UpdatedTicketValidator(TicketServiceContext context)
    {
        _context = context;
        ArgumentNullException.ThrowIfNull(context);

        RuleFor(u => u.ParentId).MustAsync(async (id, c) =>
        {
            if (!id.HasValue)
            {
                return true;
            }

            return await context.Tickets.FindAsync(id, c).ConfigureAwait(false) != null;
        });
        RuleFor(u => u.Name).NotEmpty();
        RuleFor(u => u.Id).MustAsync(async (id, c) =>
        {
            var ticketExists = await context.Tickets.FindAsync(id, c).ConfigureAwait(false) != null;
            return ticketExists;
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
        // TODO
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