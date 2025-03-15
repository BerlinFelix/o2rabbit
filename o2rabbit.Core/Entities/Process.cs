namespace o2rabbit.Core.Entities;

public class Process
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Process> SubProcesses { get; } = [];

    public List<Process> PossibleParentProcesses { get; } = [];

    public List<ProcessComment> Comments { get; } = [];

    public List<Space> PossibleSpaces { get; } = [];

    public List<Ticket> Tickets { get; } = [];
}