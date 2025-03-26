namespace o2rabbit.Core.Entities;

public class Workflow
{
    public long Id { get; set; }

    public List<Status> Statuses { get; } = [];

    public long ProcessId { get; set; }
    public Process? Process { get; set; }
}