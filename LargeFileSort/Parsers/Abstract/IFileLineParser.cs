using LargeFileSort.Entities;

namespace LargeFileSort.Parsers.Abstract;

public interface IFileLineParser<T> where T : IFileLine
{
    T Parse(string line);
    
    bool TryParse(string line, out T fileLine);
}