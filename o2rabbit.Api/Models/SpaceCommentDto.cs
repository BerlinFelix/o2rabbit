namespace o2rabbit.Api.Models;

public class SpaceCommentDto

{
    public long Id { get; set; }

    public required string Text { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public long SpaceId { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public bool IsPinned { get; set; }
}