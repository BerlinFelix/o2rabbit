using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Spaces;

internal partial class SpaceService
{
    public async Task<Result<Space>> UpdateAsync(UpdateSpaceCommand update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
            return Result.Fail(new NullInputError());

        try
        {
            var validationResult = _spaceValidator.ValidateUpdatedSpace(update);

            if (!validationResult.IsValid)
                return Result.Fail(new ValidationNotSuccessfulError(validationResult));

            var updatedRows = await _context.Spaces
                .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(s => s.Title, update.Title)
                            .SetProperty(s => s.Description, update.Description), cancellationToken
                ).ConfigureAwait(false);

            var space = await _context.Spaces
                .FindAsync(update.Id, cancellationToken).ConfigureAwait(false);
            return Result.Ok(space);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            if (e is AggregateException aggregateException)
                _logger.LogAggregateException(aggregateException);

            return Result.Fail<Space>(new UnknownError());
        }
    }
}