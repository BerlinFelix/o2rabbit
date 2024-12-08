using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpPost]
    public async Task<ActionResult<DefaultTicketDto>> CreateAsync(NewTicketCommand? newTicket,
        CancellationToken cancellationToken = default)
    {
        if (newTicket is null) return BadRequest("Ticket is null");

        var result = await _ticketService.CreateAsync(newTicket, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            var dto = result.Value.ToDefaultDto();
            return Ok(dto);
        }

        if (result.HasError<ValidationNotSuccessfulError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
    }
}