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
            Parent = ticket.Parent?.ToRelatedTicketDto(),
        };

        dto.Comments.AddRange(ticket.Comments.Select(c => c.ToDto()));

        dto.Children.AddRange(ticket.Children.Select(c => c.ToRelatedTicketDto()));

        return dto;
    }

    public static RelatedTicketDto ToRelatedTicketDto(this Ticket ticket)
    {
        return new RelatedTicketDto()
        {
            Id = ticket.Id,
            Name = ticket.Name,
        };
    }
}