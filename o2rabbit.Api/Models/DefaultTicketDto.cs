namespace o2rabbit.Api.Models;

public class DefaultTicketDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<TinyTicketDto> Children { get; } = [];

    public TinyTicketDto? Parent { get; set; }

    public long? ProcessId { get; set; }

    public List<DefaultCommentDto> Comments { get; } = [];
}