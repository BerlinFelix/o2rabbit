using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Tickets;

public class TicketValidator : ITicketValidator
{
    private readonly IValidator<NewTicketCommand> _newTicketValidator;
    private readonly IValidator<UpdateTicketCommand> _ticketUpdateValidator;

    public TicketValidator(IValidator<NewTicketCommand> newTicketValidator,
        IValidator<UpdateTicketCommand> ticketUpdateValidator)
    {
        ArgumentNullException.ThrowIfNull(newTicketValidator);
        ArgumentNullException.ThrowIfNull(ticketUpdateValidator);

        _newTicketValidator = newTicketValidator;
        _ticketUpdateValidator = ticketUpdateValidator;
    }

    public Task<ValidationResult> ValidateAsync(NewTicketCommand newTicket,
        CancellationToken cancellationToken = default)
        => _newTicketValidator.ValidateAsync(newTicket, cancellationToken);

    public Task<ValidationResult> ValidateAsync(UpdateTicketCommand update,
        CancellationToken cancellationToken = default)
        => _ticketUpdateValidator.ValidateAsync(update, cancellationToken);
}