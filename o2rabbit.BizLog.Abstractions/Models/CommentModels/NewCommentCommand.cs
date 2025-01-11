namespace o2rabbit.BizLog.Abstractions.Models.CommentModels;

public class NewCommentCommand
{
    public required string Text { get; set; } = string.Empty;

    public required long TicketId { get; set; }
}