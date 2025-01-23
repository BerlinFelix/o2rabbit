using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class TicketService
{
    public async Task<Result<Ticket>> GetByIdAsync(long id, GetTicketByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Ticket? ticket;
            var query = _context.Tickets.AsQueryable();
            if (options is { IncludeParent: true })
            {
                query = query
                    .Include(p => p.Parent);
            }

            if (options is { IncludeChildren: true })
            {
                query = query
                    .Include(p => p.Children);
            }

            if (options is { IncludeComments: true })
            {
                query = query.Include(p => p.Comments);
            }

            ticket = await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
                .ConfigureAwait(false);

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

    public async Task<Result<List<Ticket>>> SearchAsync(SearchOptions options,
        CancellationToken cancellationToken = default)
    {
        if (_searchOptionsValidator.Validate(null, options).Failed)
        {
            return Result.Fail(new InvalidInputError());
        }

        try
        {
            var results = await _context.Tickets
                .Where(t => t.Name.Contains(options.SearchText))
                .OrderBy(t => t.Id)
                .Skip((options.Page - 1) * options.PageSize)
                .Take(options.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return results;
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