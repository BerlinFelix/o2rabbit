using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Abstractions;

public interface ITicketValidator
{
    Task<ValidationResult> ValidateAsync(NewTicketCommand newTicket, CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(UpdatedTicketCommand update,
        CancellationToken cancellationToken = default);
}