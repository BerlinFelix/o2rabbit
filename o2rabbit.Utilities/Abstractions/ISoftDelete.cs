namespace o2rabbit.Utilities.Abstractions;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public void UndoDeletion()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}