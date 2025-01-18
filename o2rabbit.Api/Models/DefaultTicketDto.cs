namespace o2rabbit.Api.Models;

public class DefaultTicketDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<ChildTicketDto> Children { get; } = [];

    public long? ParentId { get; set; }

    public long? ProcessId { get; set; }

    public List<DefaultCommentDto> Comments { get; } = [];
}