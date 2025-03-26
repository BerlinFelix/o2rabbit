namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels;

public class NewProcessCommand
{
    public class NewStatusCommand
    {
        public long TempId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
    }

    public class NewStatusTransitionCommand
    {
        public string Name { get; set; } = string.Empty;

        public long FromStatusTempId { get; set; }

        public long ToStatusTempId { get; set; }
    }

    public class NewWorkflowCommand
    {
        public string Name { get; set; } = string.Empty;

        public List<NewStatusCommand> StatusList { get; } = [];

        public List<NewStatusTransitionCommand> StatusTransitionList { get; } = [];
    }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<long> SubProcessIds { get; } = [];

    public List<long> PossibleSpaceIds { get; } = [];

    public required NewWorkflowCommand Workflow { get; set; }
}