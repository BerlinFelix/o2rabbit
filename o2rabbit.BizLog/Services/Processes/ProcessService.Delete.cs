using FluentResults;
using Microsoft.EntityFrameworkCore;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deletedRows = await _context.Processes.Where(p => p.Id == id)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            if (deletedRows == 0)
            {
                return Result.Fail(new InvalidIdError());
            }

            return Result.Ok();
        }
        catch (Exception e)
        {
            LoggerExtensions.CustomExceptionLogging(_logger, e);
            return Result.Fail(new UnknownError());
        }
    }
}