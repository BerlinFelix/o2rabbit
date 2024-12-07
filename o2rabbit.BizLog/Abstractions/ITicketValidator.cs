using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models;
using o2rabbit.BizLog.Models;

namespace o2rabbit.BizLog.Abstractions;

public interface ITicketValidator
{
    Task<ValidationResult> ValidateAsync(NewTicketDto newTicket, CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(TicketUpdate ticketUpdate,
        CancellationToken cancellationToken = default);
}