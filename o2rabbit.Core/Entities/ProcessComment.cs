namespace o2rabbit.Core.Entities;

public class ProcessComment
{
    public long Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.MinValue;

    public Process? Process { get; set; }

    public long ProcessId { get; set; }

    public bool IsPinned { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}