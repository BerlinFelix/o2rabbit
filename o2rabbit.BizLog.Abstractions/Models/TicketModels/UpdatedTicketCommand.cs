namespace o2rabbit.BizLog.Abstractions.Models.TicketModels;

public class UpdatedTicketCommand
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public long? ParentId { get; set; }
}