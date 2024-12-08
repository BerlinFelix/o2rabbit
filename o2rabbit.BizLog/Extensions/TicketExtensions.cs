using o2rabbit.BizLog.Abstractions.Models.TicketModels;
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
            ParentId = dto.ParentId,
        };

    public static DefaultTicketDto ToDefaultDto(this Ticket ticket)
    {
        var dto = new DefaultTicketDto()
        {
            Id = ticket.Id,
            Name = ticket.Name,
            ProcessId = ticket.ProcessId,
            ParentId = ticket.ParentId,
        };

        dto.ChildrenIds.AddRange(ticket.Children.Select(c => c.Id));

        return dto;
    }
}