using FluentResults;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using LoggerExtensions = Microsoft.Extensions.Logging.LoggerExtensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> UpdateAsync(Process process, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProcess =
                await _context.Processes.FindAsync(process.Id, cancellationToken).ConfigureAwait(false);
            if (existingProcess == null)
                return Result.Fail<Process>(new InvalidIdError());
            _context.Update(existingProcess).CurrentValues.SetValues(process);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok<Process>(existingProcess);
        }
        catch (Exception e)
        {
            LoggerExtensions.LogError(_logger, e, e.Message);
            if (e is AggregateException aggregateException)
                Utilities.Extensions.LoggerExtensions.LogAggregateException(_logger, aggregateException);

            return Result.Fail<Process>(new UnknownError());
        }
    }
}