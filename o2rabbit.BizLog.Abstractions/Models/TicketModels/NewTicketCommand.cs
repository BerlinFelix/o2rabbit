namespace o2rabbit.BizLog.Abstractions.Models.TicketModels;

public class NewTicketCommand
{
    public required string Name { get; set; }

    public long? ParentId { get; set; }

    public long? ProcessId { get; set; }
}