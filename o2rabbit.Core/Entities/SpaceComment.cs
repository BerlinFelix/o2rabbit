namespace o2rabbit.Core.Entities;

public class SpaceComment
{
    public long Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.MinValue;

    public Space Space { get; set; }

    public long SpaceId { get; set; }

    public bool IsPinned { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}