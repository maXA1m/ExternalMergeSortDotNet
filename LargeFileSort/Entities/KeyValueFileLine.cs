namespace LargeFileSort.Entities;

public class KeyValueFileLine : IFileLine
{
    private const string Separator = ". ";

    // public int Id =>
    //     int.Parse(OriginalLine.Substring(0, OriginalLine.IndexOf(Separator, 1, StringComparison.Ordinal) + 1));
    
    // public string Value => 
    //     OriginalLine.Substring(OriginalLine.IndexOf(Separator, 1, StringComparison.Ordinal) + 1);
    
    public string OriginalLine { get; init; }

    public static int AverageSizeBytes => 200;

    public int CompareTo(IFileLine other)
    {
        if (other is not KeyValueFileLine line)
        {
            return 1;
        }
        
        var thisLine = OriginalLine;
        var otherLine = line.OriginalLine;

        var thisSeparatorId = thisLine.IndexOf(Separator, 1, StringComparison.Ordinal);
        var otherSeparatorId = otherLine.IndexOf(Separator, 1, StringComparison.Ordinal);

        // Use Span to avoid allocating more strings during Compare
        var thisValue = thisLine.AsSpan(thisSeparatorId + Separator.Length);
        var otherValue = otherLine.AsSpan(otherSeparatorId + Separator.Length);

        var result = thisValue.CompareTo(otherValue, StringComparison.Ordinal);
        if (result != 0)
        {
            return result;
        }

        var thisId = int.Parse(thisLine.AsSpan(0, thisSeparatorId));
        var otherId = int.Parse(otherLine.AsSpan(0, otherSeparatorId));
        
        return thisId.CompareTo(otherId);
    }

    public override string ToString() => OriginalLine;
}