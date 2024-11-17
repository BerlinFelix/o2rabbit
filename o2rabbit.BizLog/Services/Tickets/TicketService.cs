using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Models;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Tickets;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal class TicketService : ITicketService
{
    private readonly TicketServiceContext _context;
    private readonly ILogger<TicketService> _logger;
    private readonly ITicketValidator _ticketValidator;

    public TicketService(TicketServiceContext context,
        ILogger<TicketService> logger,
        ITicketValidator ticketValidator)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(ticketValidator);

        _context = context;
        _logger = logger;
        _ticketValidator = ticketValidator;
    }

    public async Task<Result<Ticket>> CreateAsync(Ticket ticket,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _ticketValidator.ValidateAsync(ticket, cancellationToken)
                .ConfigureAwait(false);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.ToString());

            await _context.Tickets.AddAsync(ticket, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok(ticket);
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

    public async Task<Result<Ticket>> UpdateAsync(Ticket update, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingTicket =
                await _context.Tickets.FindAsync(update.Id, cancellationToken).ConfigureAwait(false);
            var ticketUpdate = new TicketUpdate(existingTicket, update);
            var validationResult =
                await _ticketValidator.ValidateAsync(ticketUpdate, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Result.Fail<Ticket>(new InvalidIdError());
            _context.Update(existingTicket).CurrentValues.SetValues(update);
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