using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class TicketService
{
    public async Task<Result<Ticket>> UpdateAsync(UpdatedTicketCommand update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
            return Result.Fail<Ticket>(new NullInputError());

        try
        {
            var validationResult =
                await _ticketValidator.ValidateAsync(update, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
                return Result.Fail<Ticket>(new ValidationNotSuccessfulError(validationResult));

            var existingTicket = await _context.Tickets.FindAsync(update.Id).ConfigureAwait(false);
            _context.Update(existingTicket!).CurrentValues.SetValues(update);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok(existingTicket!);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            if (e is AggregateException aggregateException)
                _logger.LogAggregateException(aggregateException);

            return Result.Fail<Ticket>(new UnknownError());
        }
    }
}