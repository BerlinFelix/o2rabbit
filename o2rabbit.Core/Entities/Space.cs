namespace o2rabbit.Core.Entities;

public class Space
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public List<Process> AssignedProcesses { get; } = [];

    public List<Ticket> AssignedTickets { get; } = [];

    public List<TicketComment> Comments { get; } = [];
}