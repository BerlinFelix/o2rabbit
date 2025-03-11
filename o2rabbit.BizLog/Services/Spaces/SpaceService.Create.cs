using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Extensions;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Spaces;

internal partial class SpaceService
{
    public async Task<Result<Space>> CreateAsync(NewSpaceCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Result.Fail(new NullInputError());

        try
        {
            var validationResult = _spaceValidator.ValidateNewSpace(command);
            if (!validationResult.IsValid)
            {
                return Result.Fail(new ValidationNotSuccessfulError(validationResult));
            }

            var space = command.ToSpace();
            _context.Add(space);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Ok(space);
        }
        catch (Exception e)
        {
            _logger.CustomExceptionLogging(e);
            return Result.Fail(new UnknownError());
        }
    }
}