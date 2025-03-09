using FluentResults;
using o2rabbit.Core.ResultErrors;
using LoggerExtensions = Microsoft.Extensions.Logging.LoggerExtensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var process = await _context.Processes.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (process == null)
            {
                return Result.Fail(new InvalidIdError());
            }
            else
            {
                _context.Processes.Remove(process);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return Result.Ok();
            }
        }
        catch (Exception e)
        {
            LoggerExtensions.LogError(_logger, e, e.Message);
            if (e is AggregateException aggregateException)
                Utilities.Extensions.LoggerExtensions.LogAggregateException(_logger, aggregateException);
            return Result.Fail(new UnknownError());
        }
    }
}