namespace o2rabbit.BizLog.Abstractions.Models.TicketModels;

public class DefaultTicketDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public List<long> ChildrenIds { get; } = [];

    public long? ParentId { get; set; }

    public long? ProcessId { get; set; }
}