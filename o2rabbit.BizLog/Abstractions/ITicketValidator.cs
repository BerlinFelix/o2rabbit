using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Abstractions;

public interface ITicketValidator
{
    Task<ValidationResult> ValidateAsync(NewTicketDto newTicket, CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(UpdatedTicketDto update,
        CancellationToken cancellationToken = default);
}