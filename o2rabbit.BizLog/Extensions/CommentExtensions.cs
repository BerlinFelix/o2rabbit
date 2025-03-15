using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

internal static class CommentExtensions
{
    public static TicketComment ToComment(this NewCommentCommand command)
    {
        var now = DateTimeOffset.UtcNow;
        var comment = new TicketComment()
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