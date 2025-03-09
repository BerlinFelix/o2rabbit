using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> CreateAsync(Process process,
        CancellationToken cancellationToken = default)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (process == null) return Result.Fail<Process>("Process is null");

        try
        {
            var existingProcess =
                await _context.Processes.FindAsync(process.Id, cancellationToken).ConfigureAwait(false);
            if (existingProcess != null)
            {
                return Result.Fail(new InvalidIdError());
            }

            await _context.Processes.AddAsync(process, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Ok(process);
        }
        catch (Exception e)
        {
            LoggerExtensions.LogError(_logger, e, e.Message);
            return Result.Fail<Process>(new UnknownError());
        }
    }
}