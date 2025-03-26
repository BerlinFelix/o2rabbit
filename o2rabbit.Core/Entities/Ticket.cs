namespace o2rabbit.Core.Entities;

public class Ticket
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<Ticket> Children { get; } = [];

    public long? ParentId { get; set; }

    public Ticket? Parent { get; set; }

    public long ProcessId { get; set; }

    public Process? Process { get; set; }

    public List<TicketComment> Comments { get; } = [];

    public long SpaceId { get; set; }

    public Space? Space { get; set; }
}