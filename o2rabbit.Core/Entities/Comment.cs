namespace o2rabbit.Core.Entities;

public class Comment
{
    public long Id { get; set; }

    public required string Text { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public Ticket? Ticket { get; set; }

    public required long TicketId { get; set; }
}