namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels.StatusModels;

public class NewStatusCommand
{
    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public long WorkflowId { get; set; }
}