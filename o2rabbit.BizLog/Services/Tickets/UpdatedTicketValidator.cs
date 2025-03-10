using FluentValidation;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

public class UpdatedTicketValidator : AbstractValidator<UpdateTicketCommand>
{
    private readonly DefaultContext _context;

    public UpdatedTicketValidator(DefaultContext context)
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
        RuleFor(u => u.ParentId).MustAsync(async (u, parentId, c) =>
        {
            if (!parentId.HasValue)
                return true;

            var isValid = true;

            var cts = new CancellationTokenSource();
            var children =
                await _context.Tickets.Where(t => t.ParentId == u.Id).ToListAsync(c).ConfigureAwait(false);
            try
            {
                await Parallel.ForEachAsync(
                    children,
                    new ParallelOptions { CancellationToken = cts.Token },
                    async (child, innerCancellationToken) =>
                    {
                        if (innerCancellationToken.IsCancellationRequested)
                            return;
                        var childTreeContainsId =
                            await ChildTreeContainsId(child, (long)u.ParentId, innerCancellationToken)
                                .ConfigureAwait(false);
                        if (childTreeContainsId)
                        {
                            isValid = false;
                            cts.Cancel();
                        }
                    }).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // Nothing to do
            }

            return isValid;
        }).WithMessage(u => $"ParentId {u.ParentId} is invalid. A circular dependency would be created.");
    }

    /// <summary>
    /// Checks if <paramref name="ticket"/> has any children in its' ticket tree with id <paramref name="id"/>.
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<bool> ChildTreeContainsId(Ticket ticket, long id,
        CancellationToken cancellationToken = default)
    {
        // TODO performance
        if (ticket.Id == id)
            return true;

        var children = await _context.Tickets
            .Where(t => t.ParentId == ticket.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var child in children)
        {
            if (await ChildTreeContainsId(child, id, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }
}