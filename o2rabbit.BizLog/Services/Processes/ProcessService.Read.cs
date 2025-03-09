using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> GetByIdAsync(long id, GetProcessByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Process? process;
            if (options != null && options.IncludeChildren)
            {
                process = await EntityFrameworkQueryableExtensions
                    .Include<Process, List<Process>>(_context.Processes, p => p.Children)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                process = await _context.Processes
                    .FindAsync(id, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (process == null)
            {
                return Result.Fail<Process>(new InvalidIdError());
            }

            return Result.Ok(process);
        }
        catch (Exception e)
        {
            LoggerExtensions.LogError(_logger, e, e.Message);
            return Result.Fail<Process>(new UnknownError());
        }
    }
}