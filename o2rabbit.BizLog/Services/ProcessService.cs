using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Context;
using o2rabbit.Core;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services;

internal class ProcessService
{
    private readonly ProcessServiceContext _context;
    private ILogger<ProcessService> _logger;

    public ProcessService(ProcessServiceContext context, ILogger<ProcessService> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

    internal async Task<Result<Process>> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Process>> CreateAsync(Process? process)
    {
        if (process == null) return Result.Fail<Process>("Process is null");

        try
        {
            var existingProcess = await _context.Processes.FindAsync(process.Id).ConfigureAwait(false);
            if (existingProcess != null)
            {
                return Result.Fail<Process>(new InvalidIdError());
            }

            await _context.Processes.AddAsync(process).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Result.Ok(process);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while creating a new process.");
            throw;
        }
    }
}