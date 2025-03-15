using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Processes;

public partial class ProcessController
{
    [HttpPut("{id}")]
    public async Task<ActionResult<DefaultProcessDto>> UpdateAsync(
        long id,
        [FromBody] UpdateProcessCommand? command,
        CancellationToken cancellationToken = default)
    {
        if (command is null) return BadRequest("Command is null");

        command.Id = id;

        var result = await _processService.UpdateAsync(command, cancellationToken).ConfigureAwait(false);

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