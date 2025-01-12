using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Tickets;

public partial class TicketController
{
    [HttpPost("{id}/comments")]
    public async Task<ActionResult<DefaultCommentDto>> CreateCommentAsync([FromBody] NewCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
            return BadRequest();

        var result = await _commentService.CreateAsync(command, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var dto = result.Value.ToDto();
            return Ok(dto);
        }

        if (result.HasError(e => e is ValidationNotSuccessfulError))
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(500);
    }
}