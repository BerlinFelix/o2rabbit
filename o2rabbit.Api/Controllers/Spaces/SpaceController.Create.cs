using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Spaces;

public partial class SpaceController
{
    [HttpPost]
    public async Task<ActionResult<DefaultSpaceDto>> CreateAsync([FromBody] NewSpaceCommand? newSpace,
        CancellationToken cancellationToken = default)
    {
        if (newSpace is null) return BadRequest("Space is null");

        var result = await _spaceService.CreateAsync(newSpace, cancellationToken).ConfigureAwait(false);
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