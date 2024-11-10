namespace o2rabbit.Core.Entities;

public class Ticket
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<Ticket> Children { get; } = [];

    public long? ParentId { get; set; }

    public Process? Parent { get; set; }

    public long ProcessId { get; set; }
}