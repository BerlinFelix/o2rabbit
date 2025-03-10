using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Comments;

internal partial class CommentService
{
    public async Task<Result<TicketComment>> CreateAsync(NewCommentCommand newCommentCommand,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newCommentCommand);

        try
        {
            var validationResult =
                await _commentValidator.ValidateNewCommentAsync(newCommentCommand).ConfigureAwait(false);

            if (!validationResult.IsValid)
                return Result.Fail(new ValidationNotSuccessfulError());

            var comment = newCommentCommand.ToComment();

            _context.TicketComments.Add(comment);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return comment;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            if (e is AggregateException aggregateException)
                _logger.LogAggregateException(aggregateException);
            return Result.Fail(new UnknownError());
        }
    }
}