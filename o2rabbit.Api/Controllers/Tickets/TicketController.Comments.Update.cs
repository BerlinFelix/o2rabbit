using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpPut("{ticketId}/comments/{id}")]
    public async Task<ActionResult<DefaultCommentDto>> UpdateAsync(
        long id,
        [FromBody] UpdateCommentCommand? command,
        CancellationToken cancellationToken = default)
    {
        if (command is null) return BadRequest("Command is null");

        command.Id = id;

        var result = await _commentService.UpdateAsync(command, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            var dto = result.Value.ToDto();
            return Ok(dto);
        }

        if (result.HasError<ValidationNotSuccessfulError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}