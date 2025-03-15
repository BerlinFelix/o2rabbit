using FluentResults;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService
{
    public async Task<Result<Process>> UpdateAsync(UpdateProcessCommand update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
            return Result.Fail(new NullInputError());

        try
        {
            var validationResult = _processValidator.ValidateUpdatedProcess(update);

            if (!validationResult.IsValid)
                return Result.Fail(new ValidationNotSuccessfulError(validationResult));

            var updatedRows = await _context.Processes
                .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(s => s.Name, update.Name)
                            .SetProperty(s => s.Description, update.Description),
                    cancellationToken
                ).ConfigureAwait(false);

            var process = await _context.Processes
                .FindAsync(update.Id, cancellationToken).ConfigureAwait(false);
            return Result.Ok(process);
        }
        catch (Exception e)
        {
            _logger.CustomExceptionLogging(e);
            return Result.Fail<Process>(new UnknownError());
        }
    }
}