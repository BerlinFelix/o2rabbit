using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class TicketService
{
    public async Task<Result<Ticket>> GetByIdAsync(long id, GetTicketByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Ticket? ticket;
            if (options != null && options.IncludeChildren)
            {
                ticket = await _context.Tickets
                    .Include(p => p.Children)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                ticket = await _context.Tickets
                    .FindAsync(id, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (ticket == null)
            {
                return Result.Fail<Ticket>(new InvalidIdError());
            }

            return Result.Ok(ticket);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Fail<Ticket>(new UnknownError());
        }
    }

    public async Task<Result<IEnumerable<Ticket>>> SearchAsync(SearchOptions options,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}