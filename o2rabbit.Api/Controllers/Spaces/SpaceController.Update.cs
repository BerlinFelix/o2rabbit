using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Spaces;

public partial class SpaceController
{
    [HttpPut("{id}")]
    public async Task<ActionResult<DefaultSpaceDto>> UpdateAsync(
        long id,
        [FromBody] UpdateSpaceCommand? command,
        CancellationToken cancellationToken = default)
    {
        if (command is null) return BadRequest("Command is null");

        command.Id = id;

        var result = await _spaceService.UpdateAsync(command, cancellationToken).ConfigureAwait(false);
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