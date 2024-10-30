namespace LargeFileSort.Entities;

public interface IFileLine : IComparable<IFileLine>
{
    public string OriginalLine { get; init; }
}