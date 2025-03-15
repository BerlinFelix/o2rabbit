namespace o2rabbit.Api.Models;

public class ProcessCommentDto
{
    public long Id { get; set; }

    public required string Text { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.MinValue;

    public long ProcessId { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public bool IsPinned { get; set; }
}