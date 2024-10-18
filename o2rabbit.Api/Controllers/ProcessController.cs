using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.Models;

namespace o2rabbit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcessController : ControllerBase
{
   [HttpGet]
   public async Task<ActionResult<Process>> GetByIdAsync(long id)
   {
      var process =  new Process{Id = id, Name = "ProcessName"};
      return Ok(process);
   }
}