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

        try
        {
            var validationResult = _processValidator.ValidateNewProcess(command);
            if (!validationResult.IsValid)
            {
                return Result.Fail(new ValidationNotSuccessfulError(validationResult));
            }

            var process = command.ToProcess();
            _context.Add(process);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok(process);
        }
        catch (Exception e)
        {
            LoggerExtensions.CustomExceptionLogging(_logger, e);
            return Result.Fail(new UnknownError());
        }
    }
}