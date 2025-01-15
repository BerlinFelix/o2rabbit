using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;

namespace o2rabbit.BizLog.InternalAbstractions;

internal interface ICommentValidator
{
    Task<ValidationResult> ValidateNewCommentAsync(NewCommentCommand command,
        CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateUpdatedComment(UpdateCommentCommand command,
        CancellationToken cancellationToken = default);
}