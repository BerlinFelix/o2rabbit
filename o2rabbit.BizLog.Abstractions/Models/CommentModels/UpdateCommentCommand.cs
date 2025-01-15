namespace o2rabbit.BizLog.Abstractions.Models.CommentModels;

public class UpdateCommentCommand
{
    public long Id { get; set; }

    public string Text { get; set; } = string.Empty;
}