using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers;

[ApiController]
[Route("api/processes")]
public class ProcessController : ControllerBase
{
    private readonly IProcessService _processService;

    public ProcessController(IProcessService processService)
    {
        ArgumentNullException.ThrowIfNull(processService);

        _processService = processService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Process>> GetByIdAsync(long id,
        [FromBody] GetByIdOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _processService.GetByIdAsync(id, options, cancellationToken).ConfigureAwait(false);

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
    public async Task<ActionResult<Process>> CreateAsync(Process? process,
        CancellationToken cancellationToken = default)
    {
        if (process is null) return BadRequest("Process is null");

        var result = await _processService.CreateAsync(process, cancellationToken).ConfigureAwait(false);
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
        var result = await _processService.DeleteAsync(id).ConfigureAwait(false);

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
    public async Task<ActionResult<Process>> UpdateAsync(Process? process,
        CancellationToken cancellationToken = default)
    {
        if (process is null) return BadRequest("Process is null");

        var result = await _processService.UpdateAsync(process, cancellationToken).ConfigureAwait(false);
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