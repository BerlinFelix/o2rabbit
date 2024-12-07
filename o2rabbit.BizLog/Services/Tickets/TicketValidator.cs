using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Models;
using o2rabbit.BizLog.Models;

namespace o2rabbit.BizLog.Services.Tickets;

public class TicketValidator : ITicketValidator
{
    private readonly IValidator<NewTicketDto> _newTicketValidator;
    private readonly IValidator<TicketUpdate> _ticketUpdateValidator;

    public TicketValidator(IValidator<NewTicketDto> newTicketValidator, IValidator<TicketUpdate> ticketUpdateValidator)
    {
        ArgumentNullException.ThrowIfNull(newTicketValidator);
        ArgumentNullException.ThrowIfNull(ticketUpdateValidator);

        _newTicketValidator = newTicketValidator;
        _ticketUpdateValidator = ticketUpdateValidator;
    }

    public Task<ValidationResult> ValidateAsync(NewTicketDto newTicket, CancellationToken cancellationToken = default)
        => _newTicketValidator.ValidateAsync(newTicket, cancellationToken);

    public Task<ValidationResult> ValidateAsync(TicketUpdate ticketUpdate,
        CancellationToken cancellationToken = default)
        => _ticketUpdateValidator.ValidateAsync(ticketUpdate, cancellationToken);
}