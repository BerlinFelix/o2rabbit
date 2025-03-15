using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers.Processes;

public partial class ProcessController
{
    [HttpPost]
    public async Task<ActionResult<DefaultProcessDto>> CreateAsync([FromBody] NewProcessCommand? command,
        CancellationToken cancellationToken = default)
    {
        if (command is null) return BadRequest("Process is null");

        var result = await _processService.CreateAsync(command, cancellationToken).ConfigureAwait(false);
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