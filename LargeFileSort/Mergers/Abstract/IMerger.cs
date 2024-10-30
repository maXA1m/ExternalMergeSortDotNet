using LargeFileSort.Entities;

namespace LargeFileSort.Mergers.Abstract;

public interface IMerger<T> where T : IFileLine, new()
{ 
    string MergeChunks(List<string> chunks, string tempFolder);
}