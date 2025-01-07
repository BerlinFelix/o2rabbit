using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Options.Search;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Controllers.Searches;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IValidator<SearchOptions> _optionsValidator;

    public SearchController(IValidator<SearchOptions> optionsValidator,
        ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(optionsValidator);
        ArgumentNullException.ThrowIfNull(optionsValidator);

        _optionsValidator = optionsValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> SearchForTicketsAsync([FromQuery] SearchOptions searchOptions)
    {
        if (!_optionsValidator.Validate(searchOptions).IsValid)
        {
            return BadRequest();
        }

        throw new NotImplementedException();
    }
}