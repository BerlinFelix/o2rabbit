using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.Api.Parameters.Tickets;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<DefaultTicketDto>> GetByIdAsync(long id,
        [FromQuery] GetTicketQueryParameters? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var options = new GetTicketByIdOptions
        {
            IncludeChildren = queryParameters?.IncludeChildren ?? false,
            IncludeComments = queryParameters?.IncludeComments ?? false,
            IncludeParent = queryParameters?.IncludeParent ?? false,
        };

        var result = await _ticketService.GetByIdAsync(id, options, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var dto = result.Value.ToDefaultDto();
            return Ok(dto);
        }

        if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
    }
}