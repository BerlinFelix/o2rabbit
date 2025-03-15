using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Comments;

public class UpdatedCommentValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdatedCommentValidator(DefaultContext defaultContext)
    {
        RuleFor(c => c.Text)
            .NotEmpty();
    }
}