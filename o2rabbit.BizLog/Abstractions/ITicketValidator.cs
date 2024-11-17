using FluentValidation.Results;
using o2rabbit.BizLog.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions;

public interface ITicketValidator
{
    Task<ValidationResult> ValidateAsync(Ticket newTicket, CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(TicketUpdate ticketUpdate,
        CancellationToken cancellationToken = default);
}