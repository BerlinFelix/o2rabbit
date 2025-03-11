using FluentResults;
using Microsoft.EntityFrameworkCore;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Services.Spaces;

internal partial class SpaceService
{
    public async Task<Result<Space>> GetByIdAsync(long id, GetSpaceByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var spaces = _context.Spaces.AsQueryable();

            if (options?.IncludeProcesses == true)
            {
                spaces = spaces.Include(s => s.AttachableProcesses);
            }

            if (options?.IncludeTickets == true)
            {
                spaces = spaces.Include(s => s.AttachedTickets);
            }

            if (options?.IncludeComments == true)
            {
                spaces = spaces.Include(s => s.Comments);
            }

            var space = await spaces
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (space == null)
                return Result.Fail(new InvalidIdError());

            return Result.Ok(space);
        }
        catch (Exception e)
        {
            _logger.CustomExceptionLogging(e);
            return Result.Fail(new UnknownError());
        }
    }
}