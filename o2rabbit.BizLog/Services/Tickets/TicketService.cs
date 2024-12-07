using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal partial class TicketService : ITicketService
{
    private readonly TicketServiceContext _context;
    private readonly ILogger<TicketService> _logger;
    private readonly ITicketValidator _ticketValidator;

    public TicketService(TicketServiceContext context,
        ILogger<TicketService> logger,
        ITicketValidator ticketValidator)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(ticketValidator);

        _context = context;
        _logger = logger;
        _ticketValidator = ticketValidator;
    }

    public Task<Result<Ticket>> CreateFromProcessAsync(Process process, CancellationToken cancellationToken = default)
    {
        //TODO
        throw new NotImplementedException();
    }
}