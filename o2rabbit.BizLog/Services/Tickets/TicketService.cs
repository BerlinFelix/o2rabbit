using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Tickets;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal partial class TicketService : ITicketService
{
    private readonly TicketServiceContext _context;
    private readonly ILogger<TicketService> _logger;
    private readonly ITicketValidator _ticketValidator;
    private readonly IValidateOptions<SearchOptions> _searchOptionsValidator;

    public TicketService(TicketServiceContext context,
        ILogger<TicketService> logger,
        ITicketValidator ticketValidator,
        IValidateOptions<SearchOptions> searchOptionsValidator)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(ticketValidator);
        ArgumentNullException.ThrowIfNull(searchOptionsValidator);

        _context = context;
        _logger = logger;
        _ticketValidator = ticketValidator;
        _searchOptionsValidator = searchOptionsValidator;
    }

    public Task<Result<Ticket>> CreateFromProcessAsync(Process process, CancellationToken cancellationToken = default)
    {
        //TODO
        throw new NotImplementedException();
    }
}