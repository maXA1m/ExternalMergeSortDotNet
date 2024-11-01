using LargeFileSort.Entities;

namespace LargeFileSort.Preparators.Abstract;

public interface IPreparator<T> where T : IFileLine, new()
{
    List<string> PrepareChunks(string path, string tempFolder);
}