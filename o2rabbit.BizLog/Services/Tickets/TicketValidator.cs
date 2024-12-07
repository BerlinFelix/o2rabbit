using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Services.Tickets;

public class TicketValidator : ITicketValidator
{
    private readonly IValidator<NewTicketDto> _newTicketValidator;
    private readonly IValidator<UpdatedTicketDto> _ticketUpdateValidator;

    public TicketValidator(IValidator<NewTicketDto> newTicketValidator,
        IValidator<UpdatedTicketDto> ticketUpdateValidator)
    {
        ArgumentNullException.ThrowIfNull(newTicketValidator);
        ArgumentNullException.ThrowIfNull(ticketUpdateValidator);

        _newTicketValidator = newTicketValidator;
        _ticketUpdateValidator = ticketUpdateValidator;
    }

    public Task<ValidationResult> ValidateAsync(NewTicketDto newTicket, CancellationToken cancellationToken = default)
        => _newTicketValidator.ValidateAsync(newTicket, cancellationToken);

    public Task<ValidationResult> ValidateAsync(UpdatedTicketDto update,
        CancellationToken cancellationToken = default)
        => _ticketUpdateValidator.ValidateAsync(update, cancellationToken);
}