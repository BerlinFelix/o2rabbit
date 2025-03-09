using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class CommentExtensions
{
    internal static DefaultCommentDto ToDto(this TicketComment ticketComment)
    {
        return new()
        {
            Id = ticketComment.Id,
            Text = ticketComment.Text,
            Created = ticketComment.Created,
            LastModified = ticketComment.LastModified,
            TicketId = ticketComment.TicketId,
            DeletedAt = ticketComment.DeletedAt,
            IsPinned = ticketComment.IsPinned,
        };
    }
}