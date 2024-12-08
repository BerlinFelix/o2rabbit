using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class TicketService
{
    public async Task<Result<Ticket>> CreateAsync(NewTicketCommand newTicket,
        CancellationToken cancellationToken = default)
    {
        if (newTicket == null)
            return Result.Fail(new NullInputError());

        try
        {
            var validationResult = await _ticketValidator.ValidateAsync(newTicket, cancellationToken)
                .ConfigureAwait(false);

            if (!validationResult.IsValid) return Result.Fail(new ValidationNotSuccessfulError(validationResult));
            var ticket = newTicket.ToTicket();
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok(ticket);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Fail<Ticket>(new UnknownError());
        }
    }
}