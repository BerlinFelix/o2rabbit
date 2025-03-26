namespace o2rabbit.Core.Entities;

public class StatusTransition
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public long FromStatusId { get; set; }

    public long ToStatusId { get; set; }

    public Status? FromStatus { get; set; }

    public Status? ToStatus { get; set; }
}