using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(ticketService);

        _ticketService = ticketService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetByIdAsync(long id,
        [FromBody] GetTicketByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _ticketService.GetByIdAsync(id, options, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateAsync(Ticket? ticket,
        CancellationToken cancellationToken = default)
    {
        if (ticket is null) return BadRequest("Ticket is null");

        var result = await _ticketService.CreateAsync(ticket, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var result = await _ticketService.DeleteAsync(id).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok();
        }
        else if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        }
    }

    [HttpPut()]
    public async Task<ActionResult<Ticket>> UpdateAsync(Ticket? ticket,
        CancellationToken cancellationToken = default)
    {
        if (ticket is null) return BadRequest("Ticket is null");

        var result = await _ticketService.UpdateAsync(ticket, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}