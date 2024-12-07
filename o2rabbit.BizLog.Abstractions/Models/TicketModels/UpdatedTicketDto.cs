namespace o2rabbit.BizLog.Abstractions.Models.TicketModels;

public class UpdatedTicketDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public long? ParentId { get; set; }
}