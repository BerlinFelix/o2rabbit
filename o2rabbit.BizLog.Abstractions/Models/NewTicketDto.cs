namespace o2rabbit.BizLog.Abstractions.Models;

public class NewTicketDto
{
    public required string Name { get; set; }

    public long? ParentId { get; set; }

    public long? ProcessId { get; set; }
}