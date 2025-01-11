using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;

namespace o2rabbit.BizLog.InternalAbstractions;

internal interface ICommentValidator
{
    Task<ValidationResult> ValidateNewCommentAsync(NewCommentCommand newCommentCommand,
        CancellationToken cancelationToken = default);
}