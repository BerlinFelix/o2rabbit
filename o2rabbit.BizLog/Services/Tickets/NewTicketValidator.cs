using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Services.Tickets;

public class NewTicketValidator : AbstractValidator<NewTicketCommand>
{
    public NewTicketValidator()
    {
        RuleFor(t => t).NotNull();
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.ProcessId).GreaterThan(0);
    }
}