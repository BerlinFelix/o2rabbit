using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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
}