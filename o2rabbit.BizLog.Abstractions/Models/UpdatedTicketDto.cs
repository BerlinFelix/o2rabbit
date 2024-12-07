namespace o2rabbit.BizLog.Abstractions.Models;

public class UpdatedTicketDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public long? ParentId { get; set; }

    public long? ProcessId { get; set; }
}