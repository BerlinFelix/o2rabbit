using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

internal static class CommentExtensions
{
    public static Comment ToComment(this NewCommentCommand command)
    {
        var now = DateTime.UtcNow;
        var comment = new Comment()
        {
            Id = 0,
            Text = command.Text,
            Created = now,
            LastModified = now,
            TicketId = command.TicketId,
        };

        return comment;
    }
}