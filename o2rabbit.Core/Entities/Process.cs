namespace o2rabbit.Core.Entities;

public class Process
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<Process> Children { get; } = [];

    public long? ParentId { get; set; }

    public Process? Parent { get; set; }

    public List<ProcessComment> Comments { get; } = [];

    public List<Space> PossibleSpaces { get; } = [];
}