using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpDelete("{ticketId}/comments/{id}")]
    public async Task<ActionResult<Comment>> DeleteCommentAsync(long id)
    {
        var result = await _commentService.DeleteAsync(id).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var dto = result.Value.ToDto();
            return Ok(dto);
        }

        if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
    }
}