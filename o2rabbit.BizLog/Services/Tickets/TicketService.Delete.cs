using FluentResults;
using Microsoft.EntityFrameworkCore;
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
            var deletedRows = await _context.Tickets.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            if (deletedRows == 0)
            {
                return Result.Fail(new InvalidIdError());
            }

            return Result.Ok();
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