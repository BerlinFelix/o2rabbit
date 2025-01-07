using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using o2rabbit.Api.Extensions;
using o2rabbit.Api.Models;
using o2rabbit.Api.Options.Search;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;

namespace o2rabbit.Api.Controllers.Searches;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IValidator<SearchQueryParameters> _optionsValidator;
    private readonly ITicketService _ticketService;

    public SearchController(IValidator<SearchQueryParameters> optionsValidator,
        ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(optionsValidator);
        ArgumentNullException.ThrowIfNull(optionsValidator);

        _optionsValidator = optionsValidator;
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefaultTicketDto>>> SearchForTicketsAsync(
        [FromQuery] SearchQueryParameters searchQueryParameters,
        CancellationToken cancellationToken = default)
    {
        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
        if (!_optionsValidator.Validate(searchQueryParameters).IsValid)
        {
            return BadRequest();
        }

        var result = await _ticketService.SearchAsync(new SearchOptions()
        {
            SearchText = searchQueryParameters.SearchText,
            Page = searchQueryParameters.Page,
            PageSize = searchQueryParameters.PageSize
        }, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var dtos = result.Value.Select(t => t.ToDefaultDto());
            return Ok(dtos);
        }

        return StatusCode(500);
    }
}