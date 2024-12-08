using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

internal static class TicketExtensions
{
    public static Ticket ToTicket(this NewTicketCommand command) =>
        new()
        {
            Id = 0,
            Name = command.Name,
            ProcessId = command.ProcessId,
            ParentId = command.ParentId,
        };
}