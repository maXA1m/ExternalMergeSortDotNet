using LargeFileSort.Entities;
using LargeFileSort.Parsers.Abstract;

namespace LargeFileSort.Parsers;

public class KeyValueFileLineParser : IFileLineParser<KeyValueFileLine>
{
    private const int MaxStringLength = 2048;

    public KeyValueFileLine Parse(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return null;
        }
        
        // Number must be of type long, support for infinite numbers is not implemented
        var dot = line.IndexOf('.');
        var num = line.Substring(0, dot);
        if (!long.TryParse(num, out var id))
        {
            return null;
        }

        // String limit can be changed
        if (line.Length - dot - 2 > MaxStringLength)
        {
            return null;
        }

        var value = line.Substring(dot + 2, line.Length - dot - 2);
        return new KeyValueFileLine(id, value);
    }

    public bool TryParse(string line, out KeyValueFileLine fileLine)
    {
        try
        {
            fileLine = Parse(line);
        }
        catch
        {
            fileLine = null;
        }
        
        return fileLine is not null;
    }
}