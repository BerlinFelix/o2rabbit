using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Services.Tickets;

public class TicketValidator : ITicketValidator
{
    private readonly IValidator<NewTicketCommand> _newTicketValidator;
    private readonly IValidator<UpdatedTicketCommand> _ticketUpdateValidator;

    public TicketValidator(IValidator<NewTicketCommand> newTicketValidator,
        IValidator<UpdatedTicketCommand> ticketUpdateValidator)
    {
        ArgumentNullException.ThrowIfNull(newTicketValidator);
        ArgumentNullException.ThrowIfNull(ticketUpdateValidator);

        _newTicketValidator = newTicketValidator;
        _ticketUpdateValidator = ticketUpdateValidator;
    }

    public Task<ValidationResult> ValidateAsync(NewTicketCommand newTicket,
        CancellationToken cancellationToken = default)
        => _newTicketValidator.ValidateAsync(newTicket, cancellationToken);

    public Task<ValidationResult> ValidateAsync(UpdatedTicketCommand update,
        CancellationToken cancellationToken = default)
        => _ticketUpdateValidator.ValidateAsync(update, cancellationToken);
}