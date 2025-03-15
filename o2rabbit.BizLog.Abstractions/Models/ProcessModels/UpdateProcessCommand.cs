namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels;

public class UpdateProcessCommand
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}