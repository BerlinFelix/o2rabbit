namespace o2rabbit.Api.Models;

public class ProcessCommentDto
{
    public long Id { get; set; }

    public required string Text { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public long ProcessId { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public bool IsPinned { get; set; }
}