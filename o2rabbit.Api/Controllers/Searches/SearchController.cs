using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Options;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Controllers.Searches;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    public SearchController()
    {
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> SearchForTicketsAsync([FromQuery] SearchOptions searchOptions)
    {
        throw new NotImplementedException();
    }
}