using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;

namespace o2rabbit.Api.Controllers.Processes;

[ApiController]
[Route("api/spaces")]
public partial class ProcessController : ControllerBase
{
    private readonly IProcessService _processService;

    public ProcessController(IProcessService processService)
    {
        ArgumentNullException.ThrowIfNull(processService);

        _processService = processService;
    }
}