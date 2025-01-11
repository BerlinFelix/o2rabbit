namespace o2rabbit.BizLog.Abstractions.Models.TicketModels;

public class UpdateTicketCommand
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public long? ParentId { get; set; }
}