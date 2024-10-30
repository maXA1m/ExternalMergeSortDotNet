using LargeFileSort.Entities;

namespace LargeFileSort.Sorts.Abstract;

public interface IFileSort<T> where T : IFileLine, new()
{ 
    string SortAndSave(string path);
}