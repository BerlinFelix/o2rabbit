using Microsoft.AspNetCore.Mvc;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Spaces;

public partial class SpaceController
{
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var result = await _spaceService.DeleteAsync(id).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok();
        }

        if (result.HasError<InvalidIdError>())
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
    }
}