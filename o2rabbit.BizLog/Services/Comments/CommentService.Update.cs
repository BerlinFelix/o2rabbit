using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Comments;

internal partial class CommentService
{
    public async Task<Result<TicketComment>> UpdateAsync(UpdateCommentCommand update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
            return Result.Fail(new NullInputError());

        try
        {
            var validationResult =
                await _commentValidator.ValidateUpdatedComment(update, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
                return Result.Fail(new ValidationNotSuccessfulError());

            var existing = await _context.Comments.FindAsync(update.Id).ConfigureAwait(false);
            _context.Update(existing!).CurrentValues.SetValues(update);
            existing.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok(existing!);
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