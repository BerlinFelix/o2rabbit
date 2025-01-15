using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Comments;

public class CommentValidator : ICommentValidator
{
    private readonly IValidator<NewCommentCommand> _newCommentValidator;
    private readonly IValidator<UpdateCommentCommand> _updateCommentValidator;

    public CommentValidator(IValidator<NewCommentCommand> newCommentValidator,
        IValidator<UpdateCommentCommand> updateCommentValidator)
    {
        ArgumentNullException.ThrowIfNull(newCommentValidator);
        ArgumentNullException.ThrowIfNull(updateCommentValidator);

        _newCommentValidator = newCommentValidator;
        _updateCommentValidator = updateCommentValidator;
    }

    public Task<ValidationResult> ValidateNewCommentAsync(NewCommentCommand command,
        CancellationToken cancellationToken = default) =>
        _newCommentValidator.ValidateAsync(command, cancellationToken);

    public Task<ValidationResult> ValidateUpdatedComment(UpdateCommentCommand commmand,
        CancellationToken cancellationToken = default) =>
        _updateCommentValidator.ValidateAsync(commmand, cancellationToken);
}