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
            Parent = ticket.Parent?.ToTinyTicketDto(),
        };

        dto.Comments.AddRange(ticket.Comments.Select(c => c.ToDto()));

        dto.Children.AddRange(ticket.Children.Select(c => c.ToTinyTicketDto()));

        return dto;
    }

    public static TinyTicketDto ToTinyTicketDto(this Ticket ticket)
    {
        return new TinyTicketDto()
        {
            Id = ticket.Id,
            Name = ticket.Name,
        };
    }
}