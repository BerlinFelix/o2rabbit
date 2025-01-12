using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class CommentExtensions
{
    internal static DefaultCommentDto ToDto(this Comment comment)
    {
        return new()
        {
            Id = comment.Id,
            Text = comment.Text,
            Created = comment.Created,
            LastModified = comment.LastModified,
            TicketId = comment.TicketId,
        };
    }
}