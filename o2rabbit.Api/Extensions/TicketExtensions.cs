using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class TicketExtensions
{
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