namespace o2rabbit.Core.Entities;

public class ProcessComment
{
    public long Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public Process? Process { get; set; }

    public long ProcessId { get; set; }

    public bool IsPinned { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}