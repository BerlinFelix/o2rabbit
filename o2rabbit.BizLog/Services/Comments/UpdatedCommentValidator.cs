using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Comments;

public class UpdatedCommentValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdatedCommentValidator(CommentServiceContext commentServiceContext)
    {
        RuleFor(c => c.Text)
            .NotEmpty();

        RuleFor(c => c.Id)
            .MustAsync(async (id, c) =>
            {
                var comment = await commentServiceContext.Comments.FindAsync(id, c).ConfigureAwait(false);
                var commentExists = comment != null;
                return commentExists;
            }).WithMessage("Comment not found");
    }
}