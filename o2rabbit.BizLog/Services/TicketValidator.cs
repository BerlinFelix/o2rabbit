using FluentResults;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services;

public class TicketValidator : ITicketValidator
{
    private readonly TicketServiceContext _context;

    public TicketValidator(TicketServiceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async ValueTask<Result> IsValidNewTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (ticket == null)
            return Result.Fail(new NullInputError());

        var existingTicketTask =
            _context.Tickets.FindAsync(ticket.Id, cancellationToken).AsTask();
        var isValidProcessIdTask = Task<bool>.Run(async () =>
        {
            if (ticket.ProcessId.HasValue)
            {
                var process = await _context.Processes.FindAsync(ticket.ProcessId.Value, cancellationToken);
                return process != null;
            }

            return true;
        });
        if (await existingTicketTask != null)
            return Result.Fail(new InvalidIdError($"Invalid ticket id: {ticket.Id}"));

        var isValidProcessId = await isValidProcessIdTask;
        if (!isValidProcessId)
        {
            return Result.Fail(new InvalidIdError($"Invalid process id: {ticket.ProcessId}"));
        }

        return Result.Ok();
    }
}