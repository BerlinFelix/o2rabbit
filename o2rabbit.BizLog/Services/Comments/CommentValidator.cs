using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Comments;

public class CommentValidator : ICommentValidator
{
    private readonly IValidator<NewCommentCommand> _newCommentValidator;

    public CommentValidator(IValidator<NewCommentCommand> newCommentValidator)
    {
        ArgumentNullException.ThrowIfNull(newCommentValidator);

        _newCommentValidator = newCommentValidator;
    }

    public Task<ValidationResult> ValidateNewCommentAsync(NewCommentCommand newCommentCommand,
        CancellationToken cancelationToken = default) =>
        _newCommentValidator.ValidateAsync(newCommentCommand, cancelationToken);
}