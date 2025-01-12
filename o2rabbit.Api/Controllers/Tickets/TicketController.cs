using Microsoft.AspNetCore.Mvc;
using o2rabbit.BizLog.Abstractions.Services;

namespace o2rabbit.Api.Controllers.Tickets;

[ApiController]
[Route("api/tickets")]
public partial class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ICommentService _commentService;

    public TicketController(ITicketService ticketService,
        ICommentService commentService)
    {
        ArgumentNullException.ThrowIfNull(ticketService);
        ArgumentNullException.ThrowIfNull(ticketService);

        _ticketService = ticketService;
        _commentService = commentService;
    }
}