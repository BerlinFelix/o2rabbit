using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpPut("{id}")]
    public async Task<ActionResult<DefaultTicketDto>> UpdateAsync(
        long id,
        [FromBody] UpdatedTicketCommand? ticket,
        CancellationToken cancellationToken = default)
    {
        if (ticket is null) return BadRequest("Ticket is null");

        ticket.Id = id;

        var result = await _ticketService.UpdateAsync(ticket, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            var dto = result.Value.ToDefaultDto();
            return Ok(dto);
        }

        if (result.HasError<ValidationNotSuccessfulError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}