namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels;

public class NewProcessCommand
{
    public class NewStatusCommand
    {
        public string Name { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
    }

    public class NewStatusTransitionCommand
    {
        public string Name { get; set; } = string.Empty;

        public string FromStatusName { get; set; } = string.Empty;

        public string ToStatusName { get; set; } = string.Empty;
    }

    public class NewWorkflowCommand
    {
        public string Name { get; set; } = string.Empty;

        public List<NewStatusCommand> Statuses { get; } = [];

        public List<NewStatusTransitionCommand> StatusTransitions { get; } = [];
    }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<long> SubProcessIds { get; } = [];

    public List<long> PossibleSpaceIds { get; } = [];

    public required NewWorkflowCommand Workflow { get; set; }
}