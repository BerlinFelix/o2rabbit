using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Models;
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
        [FromQuery] bool includeChildren = false,
        CancellationToken cancellationToken = default)
    {
        var options = new GetTicketByIdOptions()
        {
            IncludeChildren = includeChildren,
        };

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
    public async Task<ActionResult<Ticket>> CreateAsync(NewTicketDto? newTicket,
        CancellationToken cancellationToken = default)
    {
        if (newTicket is null) return BadRequest("Ticket is null");

        var result = await _ticketService.CreateAsync(newTicket, cancellationToken).ConfigureAwait(false);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<Ticket>> UpdateAsync(
        long id,
        [FromBody] Ticket? ticket,
        CancellationToken cancellationToken = default)
    {
        if (ticket is null) return BadRequest("Ticket is null");

        ticket.Id = id;

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