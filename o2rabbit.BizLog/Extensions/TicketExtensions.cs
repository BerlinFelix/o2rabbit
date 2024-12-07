using o2rabbit.BizLog.Abstractions.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

internal static class TicketExtensions
{
    public static Ticket ToTicket(this NewTicketDto dto) =>
        new()
        {
            Id = 0,
            Name = dto.Name,
            ProcessId = dto.ProcessId,
        };
}