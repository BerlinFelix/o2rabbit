using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;
using o2rabbit.Core.ResultErrors;

namespace o2rabbit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessController : ControllerBase
{
    private readonly IProcessService _processService;

    public ProcessController(IProcessService processService)
    {
        ArgumentNullException.ThrowIfNull(processService);

        _processService = processService;
    }

    [HttpGet]
    public async Task<ActionResult<Process>> GetByIdAsync(long id)
    {
        var process = new Process { Id = id, Name = "ProcessName" };
        return Ok(process);
    }

    public async Task<ActionResult<Process>> CreateAsync(Process? process,
        CancellationToken cancellationToken = default)
    {
        if (process is null) return BadRequest("Process is null");

        var result = await _processService.CreateAsync(process, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            return result.Value;
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

    public async Task<ActionResult> DeleteAsync(int i)
    {
        throw new NotImplementedException();
    }
}