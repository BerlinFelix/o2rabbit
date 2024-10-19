using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core;
using o2rabbit.Core.Entities;

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
      var process =  new Process{Id = id, Name = "ProcessName"};
      return Ok(process);
   }

   public async Task<ActionResult<Process>> CreateAsync(Process process, CancellationToken cancellationToken = default)
   {
      if (process is null) return BadRequest("Process is null");
      
      var result = await _processService.CreateAsync(process, cancellationToken).ConfigureAwait(false);
      if (result.IsSuccess)
      {
         return Ok(result.Value);
      }
      else
      {
         return BadRequest(result.Errors);
      }
   }
}