namespace LargeFileSort.Entities;

public class KeyValueFileLine : IFileLine
{
    public KeyValueFileLine(long id, string value)
    {
        Id = id;
        Value = value;
    }
    
    public long Id { get; init;  }
    
    public string Value { get; init; }
    
    public int CompareTo(IFileLine other)
    {
        if (other is not KeyValueFileLine line)
        {
            return 1;
        }

        var valueCompare = Value.CompareTo(line.Value);
        if (valueCompare != 0)
        {
            return valueCompare;
        }

        return Id.CompareTo(line.Id);
    }

    public override string ToString()
    {
        return $"{Id}. {Value}";
    }
}