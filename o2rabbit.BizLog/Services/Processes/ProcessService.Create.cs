using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> CreateAsync(NewProcessCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Result.Fail(new NullInputError());

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var validationResult = _processValidator.ValidateNewProcess(command);
            if (!validationResult.IsValid)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                return Result.Fail(new ValidationNotSuccessfulError(validationResult));
            }

            var process = await AddAndSaveProcess(command, cancellationToken).ConfigureAwait(false);
            var newWorkflow = await AddAndSaveWorkflow(cancellationToken, process).ConfigureAwait(false);
            var newStatuses =
                await AddAndSaveNewStatuses(command, cancellationToken, newWorkflow).ConfigureAwait(false);
            var newStatusTransitions = await AddAndSaveStatusTransitions(command, cancellationToken, newStatuses)
                .ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok(process);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            LoggerExtensions.CustomExceptionLogging(_logger, e);
            return Result.Fail(new UnknownError());
        }
    }

    private async Task<List<StatusTransition>> AddAndSaveStatusTransitions(NewProcessCommand command,
        CancellationToken cancellationToken,
        List<Status> newStatuses)
    {
        var newStatusTransitions = command.Workflow.StatusTransitions.Select(t => new StatusTransition
        {
            Name = t.Name,
            FromStatusId = newStatuses.Single(s => s.Name == t.FromStatusName).Id,
            ToStatusId = newStatuses.Single(s => s.Name == t.ToStatusName).Id,
        }).ToList();
        _context.StatusTransitions.AddRange(newStatusTransitions);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return newStatusTransitions;
    }

    private async Task<List<Status>> AddAndSaveNewStatuses(NewProcessCommand command,
        CancellationToken cancellationToken,
        Workflow newWorkflow)
    {
        var newStatuses = command.Workflow.Statuses.Select(s => new Status
        {
            Name = s.Name,
            WorkflowId = newWorkflow.Id,
            IsFinal = s.IsFinal,
        }).ToList();
        _context.AddRange(newStatuses);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return newStatuses;
    }

    private async Task<Workflow> AddAndSaveWorkflow(CancellationToken cancellationToken, Process process)
    {
        var newWorkflow = new Workflow
        {
            ProcessId = process.Id,
        };
        _context.Add(newWorkflow);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return newWorkflow;
    }

    private async Task<Process> AddAndSaveProcess(NewProcessCommand command, CancellationToken cancellationToken)
    {
        var process = command.ToProcess();
        _context.Add(process);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return process;
    }
}