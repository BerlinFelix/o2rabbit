using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;

namespace o2rabbit.Api.Controllers.Spaces;

[ApiController]
[Route("api/spaces")]
public partial class SpaceController : ControllerBase
{
    private readonly ISpaceService _spaceService;

    public SpaceController(ISpaceService spaceService)
    {
        ArgumentNullException.ThrowIfNull(spaceService);

        _spaceService = spaceService;
    }
}