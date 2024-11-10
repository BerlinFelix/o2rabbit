using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal class TicketService : ITicketService
{
    private readonly TicketServiceContext _context;
    private readonly ILogger<TicketService> _logger;

    public TicketService(TicketServiceContext context, ILogger<TicketService> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

    public async Task<Result<Ticket>> CreateAsync(Ticket Ticket,
        CancellationToken cancellationToken = default)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Ticket == null) return Result.Fail<Ticket>("Ticket is null");

        try
        {
            var existingTicket =
                await _context.Tickets.FindAsync(Ticket.Id, cancellationToken).ConfigureAwait(false);
            if (existingTicket != null)
            {
                return Result.Fail(new InvalidIdError());
            }

            await _context.Tickets.AddAsync(Ticket, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok(Ticket);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Fail<Ticket>(new UnknownError());
        }
    }

    public async Task<Result<Ticket>> GetByIdAsync(long id, GetTicketByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Ticket? Ticket;
            if (options != null && options.IncludeChildren)
            {
                Ticket = await _context.Tickets
                    .Include(p => p.Children)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                Ticket = await _context.Tickets
                    .FindAsync(id, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (Ticket == null)
            {
                return Result.Fail<Ticket>(new InvalidIdError());
            }

            return Result.Ok(Ticket);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Fail<Ticket>(new UnknownError());
        }
    }

    public async Task<Result<Ticket>> UpdateAsync(Ticket Ticket, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingTicket =
                await _context.Tickets.FindAsync(Ticket.Id, cancellationToken).ConfigureAwait(false);
            if (existingTicket == null)
                return Result.Fail<Ticket>(new InvalidIdError());
            _context.Update(existingTicket).CurrentValues.SetValues(Ticket);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok(existingTicket);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            if (e is AggregateException aggregateException)
                _logger.LogAggregateException(aggregateException);

            return Result.Fail<Ticket>(new UnknownError());
        }
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var Ticket = await _context.Tickets.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (Ticket == null)
            {
                return Result.Fail(new InvalidIdError());
            }
            else
            {
                _context.Tickets.Remove(Ticket);
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

    public Task<Result<Ticket>> CreateFromProcessAsync(Process process, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}