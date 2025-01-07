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
    private readonly ITicketService _ticketService;

    public SearchController(IValidator<SearchOptions> optionsValidator,
        ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(optionsValidator);
        ArgumentNullException.ThrowIfNull(optionsValidator);

        _optionsValidator = optionsValidator;
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> SearchForTicketsAsync([FromQuery] SearchOptions searchOptions,
        CancellationToken cancellationToken = default)
    {
        if (!_optionsValidator.Validate(searchOptions).IsValid)
        {
            return BadRequest();
        }

        var result = await _ticketService.SearchAsync(new BizLog.Abstractions.Options.SearchOptions()
        {
            SearchText = searchOptions.SearchText,
            Page = searchOptions.Page,
            PageSize = searchOptions.PageSize
        }, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(500);
    }
}