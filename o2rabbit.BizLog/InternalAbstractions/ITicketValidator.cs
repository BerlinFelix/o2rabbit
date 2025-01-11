using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.InternalAbstractions;

internal interface ITicketValidator
{
    Task<ValidationResult> ValidateAsync(NewTicketCommand newTicket, CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(UpdateTicketCommand update,
        CancellationToken cancellationToken = default);
}