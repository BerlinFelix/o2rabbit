namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels.StatusTransitionModels;

public class NewStatusTransitionCommand
{
    public string Name { get; set; } = string.Empty;

    public long FromStatusId { get; set; }

    public long ToStatusId { get; set; }
}