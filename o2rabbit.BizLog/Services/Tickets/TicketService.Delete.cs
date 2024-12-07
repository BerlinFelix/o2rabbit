using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class TicketService
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (ticket == null)
            {
                return Result.Fail(new InvalidIdError());
            }
            else
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return Result.Ok();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            if (e is AggregateException aggregateException)
                _logger.LogAggregateException(aggregateException);
            return Result.Fail(new UnknownError());
        }
    }
}