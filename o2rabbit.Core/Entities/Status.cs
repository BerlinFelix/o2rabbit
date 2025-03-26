namespace o2rabbit.Core.Entities;

public class Status
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public long WorkflowId { get; set; }

    public Workflow? Workflow { get; set; }

    public List<StatusTransition> FromTransitions { get; } = [];

    public List<StatusTransition> ToTransitions { get; } = [];

    //TODO configure in db etc.
    // public long InitialStatusId { get; set; }
    //
    // public Status? InitialStatus { get; set; }
}