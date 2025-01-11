namespace o2rabbit.BizLog.Abstractions.Models.CommentModels;

public class UpdateCommentCommand
{
    public long Id { get; set; }

    public required string Name { get; set; }
}