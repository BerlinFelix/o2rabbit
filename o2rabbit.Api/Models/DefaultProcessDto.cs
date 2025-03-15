namespace o2rabbit.Api.Models;

public class DefaultProcessDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TinyProcessDto> SubProcesses { get; } = [];

    public List<TinyProcessDto> PossibleParentProcesses { get; } = [];

    public List<ProcessCommentDto> Comments { get; } = [];

    public List<TinySpaceDto> PossibleSpaces { get; } = [];
}