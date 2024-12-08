namespace o2rabbit.Utilities.Contracts;

public interface IHierarchicalEntity<T> where T : IHasId
{
    public long? ParentId { get; }

    public T? Parent { get; }

    public List<T> Children { get; }
}