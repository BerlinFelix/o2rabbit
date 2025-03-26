using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Tickets;

public class NewTicketValidator : AbstractValidator<NewTicketCommand>
{
    private readonly DefaultContext _context;

    public NewTicketValidator(DefaultContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        RuleFor(t => t).NotNull();
        RuleFor(t => t.Name).NotEmpty();
    }
}