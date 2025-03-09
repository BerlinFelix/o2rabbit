using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Comments;

internal partial class CommentService
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id < 1)
            return Result.Fail(new InvalidIdError());

        try
        {
            var comment = await _context.Comments.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (comment is null)
                return Result.Fail(new InvalidIdError());

            comment.Text = string.Empty;

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok();
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