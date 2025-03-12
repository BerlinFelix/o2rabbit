using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Spaces;

public partial class SpaceController
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<DefaultTicketDto>> GetByIdAsync(long id,
        [FromQuery] GetSpaceQueryParameters? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var options = new GetSpaceByIdOptions
        {
            IncludeComments = queryParameters?.IncludeComments ?? false,
            IncludeProcesses = queryParameters?.IncludeProcesses ?? false,
            IncludeTickets = queryParameters?.IncludeTickets ?? false,
        };

        var result = await _spaceService.GetByIdAsync(id, options, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var dto = result.Value.ToDefaultDto();
            return Ok(dto);
        }

        if (result.HasError<InvalidIdError>())
        {
            return NotFound(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
    }
}