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

        dto.Comments.AddRange(ticket.Comments.Select(c => c.ToDto()));

        dto.Children.AddRange(ticket.Children.Select(c => c.ToChildDto()));

        return dto;
    }

    public static ChildTicketDto ToChildDto(this Ticket ticket)
    {
        return new ChildTicketDto()
        {
            Id = ticket.Id,
            Name = ticket.Name,
        };
    }
}