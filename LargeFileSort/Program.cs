using LargeFileSort.Entities;
using LargeFileSort.Mergers;
using LargeFileSort.Preparators;
using LargeFileSort.Sorts;
using LargeFileSort.Sorts.Abstract;

namespace LargeFileSort;

class Program
{
    static void Main(string[] args)
    {
        var path = Console.ReadLine();
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        
        var result = BuildSorter<KeyValueFileLine>().SortAndSave(path);
        
        Console.WriteLine(result);
    }

    private static IFileSort<T> BuildSorter<T>() where T : IFileLine, new()
    {
        // Heap merger is better
        var heapMerger = new HeapMerger<T>();
        var twoWayMerger = new TwoWayRecursiveMerger<T>();

        // Large is good for ~10Gb file
        const long chunkSmallSizeBytes = 100 * 1000000; // 100Mb
        const long chunkMediumSizeBytes = 500 * 1000000; // 500Mb
        const long chunkLargeSizeBytes = 1000 * 1000000; // 1Gb
        
        var preparator = new FixedSizeChunkPreparator<T>(KeyValueFileLine.AverageSizeBytes, chunkLargeSizeBytes);
        
        return new ExternalMergeFileSort<T>(heapMerger, preparator);
    }
}