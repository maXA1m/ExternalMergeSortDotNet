using LargeFileSort.Entities;

namespace LargeFileSort.Sorters.Abstract;

public interface IFileSorter<T> where T : IFileLine
{ 
    string SortAndSave(string path);
}