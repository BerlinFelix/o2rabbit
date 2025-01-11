using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Comments;

public class NewCommentValidator : AbstractValidator<NewCommentCommand>
{
    public NewCommentValidator(CommentServiceContext commentServiceContext)
    {
        RuleFor(c => c.Text)
            .NotEmpty();

        RuleFor(c => c.TicketId)
            .MustAsync(async (id, c) =>
            {
                //TODO context is disposable. How to dispose of it?
                var ticket = await commentServiceContext.Tickets.FindAsync(id, c).ConfigureAwait(false);
                var ticketExists = ticket != null;
                return ticketExists;
            }).WithMessage("Ticket not found");
    }
}