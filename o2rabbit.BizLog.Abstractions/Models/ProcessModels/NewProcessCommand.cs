namespace o2rabbit.BizLog.Abstractions.Models.ProcessModels;

public class NewProcessCommand
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<long> SubProcessIds { get; } = [];

    public List<long> PossibleSpaceIds { get; } = [];
}