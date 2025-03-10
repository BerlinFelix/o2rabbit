namespace o2rabbit.BizLog.Abstractions.Models.SpaceModels;

public class UpdateSpaceCommand
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}