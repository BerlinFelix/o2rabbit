using FluentResults;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions;

internal interface ITicketValidator
{
    ValueTask<Result> IsValidNewTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
}