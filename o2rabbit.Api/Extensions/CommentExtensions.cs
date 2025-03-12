using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class CommentExtensions
{
    internal static ProcessCommentDto ToDto(this ProcessCommentDto comment)
    {
        return new()
        {
            Id = comment.Id,
            Text = comment.Text,
            Created = comment.Created,
            LastModified = comment.LastModified,
            ProcessId = comment.ProcessId,
            DeletedAt = comment.DeletedAt,
            IsPinned = comment.IsPinned,
        };
    }

    internal static SpaceCommentDto ToDto(this SpaceComment comment)
    {
        return new()
        {
            Id = comment.Id,
            Text = comment.Text,
            Created = comment.Created,
            LastModified = comment.LastModified,
            SpaceId = comment.SpaceId,
            DeletedAt = comment.DeletedAt,
            IsPinned = comment.IsPinned,
        };
    }

    internal static TicketCommentDto ToDto(this TicketComment ticketComment)
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