using FluentResults;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> GetByIdAsync(long id, GetProcessByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var processes = _context.Processes.AsQueryable();

            if (options?.IncludeComments == true)
            {
                processes = processes.Include(p => p.Comments);
            }

            if (options?.IncludeTickets == true)
            {
                processes = processes.Include(p => p.AttachedTickets);
            }

            if (options?.IncludeSubProcesses == true)
            {
                processes = processes.Include(p => p.SubProcesses);
            }

            if (options?.IncludePossibleParentProcesses == true)
            {
                processes = processes.Include(p => p.PossibleParentProcesses);
            }

            var process = await processes
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (process == null)
                return Result.Fail(new InvalidIdError());

            return Result.Ok(process);
        }
        catch (Exception e)
        {
            LoggerExtensions.CustomExceptionLogging(_logger, e);
            return Result.Fail(new UnknownError());
        }
    }
}