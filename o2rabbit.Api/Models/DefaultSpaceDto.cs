namespace o2rabbit.Api.Models;

public class DefaultSpaceDto
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.MinValue;

    public List<TinyProcessDto> AttachableProcesses { get; } = [];

    public List<TinyTicketDto> AttachedTickets { get; } = [];

    public List<SpaceCommentDto> Comments { get; } = [];
}