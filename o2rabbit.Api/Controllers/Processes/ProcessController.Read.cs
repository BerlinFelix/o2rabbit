using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.Api.Parameters.Processes;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Processes;

public partial class ProcessController
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<DefaultTicketDto>> GetByIdAsync(long id,
        [FromQuery] GetProcessByIdQueryParameters? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var options = new GetProcessByIdOptions()
        {
            IncludeComments = queryParameters?.IncludeComments ?? false,
            IncludeTickets = queryParameters?.IncludeTickets ?? false,
            IncludeSubProcesses = queryParameters?.IncludeSubProcesses ?? false,
            IncludePossibleParentProcesses = queryParameters?.IncludePossibleParentProcesses ?? false,
        };

        var result = await _processService.GetByIdAsync(id, options, cancellationToken).ConfigureAwait(false);

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