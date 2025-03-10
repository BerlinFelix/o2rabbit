namespace o2rabbit.Core.Entities;

public class Space
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.MinValue;

    public List<Process> AttachableProcesses { get; } = [];

    public List<Ticket> AttachedTickets { get; } = [];

    public List<SpaceComment> Comments { get; } = [];
}