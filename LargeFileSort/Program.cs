using LargeFileSort.Entities;
using LargeFileSort.Mergers;
using LargeFileSort.Parsers;
using LargeFileSort.Parsers.Abstract;
using LargeFileSort.Preparators;
using LargeFileSort.Sorters;
using LargeFileSort.Sorters.Abstract;

namespace LargeFileSort;

class Program
{
    static void Main(string[] args)
    {
        var path = Console.ReadLine();
        
        var parser = new KeyValueFileLineParser();
        var result = BuildSorter(parser).SortAndSave(path);
        
        Console.WriteLine(result);
    }

    private static IFileSorter<T> BuildSorter<T>(IFileLineParser<T> parser) where T : IFileLine
    {
        var merger = new HeapMerger<T>(parser);
        //var merger = new RecursiveMerger<T>(parser);
        var preparator = new FixedSizeChunkPreparator<T>(parser);
        
        return new ExternalMergeFileSorter<T>(merger, preparator);
    }
}