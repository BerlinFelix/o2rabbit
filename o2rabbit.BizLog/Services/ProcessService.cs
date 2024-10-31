using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options.ProcessService;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.BizLog.Services;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal class ProcessService : IProcessService
{
    private readonly ProcessServiceContext _context;
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(ProcessServiceContext context, ILogger<ProcessService> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

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
            _logger.LogError(e, e.Message);
            return Result.Fail<Process>(new UnknownError());
        }
    }

    public async Task<Result<Process>> GetByIdAsync(long id, GetByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Process? process;
            if (options != null && options.IncludeChildren)
            {
                process = await _context.Processes
                    .Include(p => p.Children)
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
            _logger.LogError(e, e.Message);
            return Result.Fail<Process>(new UnknownError());
        }
    }
}