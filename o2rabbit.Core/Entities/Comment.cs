namespace o2rabbit.Core.Entities;

public class Comment
{
    public long Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public Ticket? Ticket { get; set; }

    public long TicketId { get; set; }

    public bool IsPinned { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}