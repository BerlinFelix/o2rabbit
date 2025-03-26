using o2rabbit.BizLog.Abstractions.Models.ProcessModels.StatusModels;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels.StatusTransitionModels;

namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels.WorkflowModels;

public class NewWorkflowCommand
{
    public string Name { get; set; } = string.Empty;

    public List<NewStatusCommand> Statuses { get; } = [];

    public List<NewStatusTransitionCommand> StatusTransitions { get; } = [];
}