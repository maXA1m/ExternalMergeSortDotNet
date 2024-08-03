using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;
using LargeFileSort.Preparators.Abstract;
using LargeFileSort.Sorters.Abstract;

namespace LargeFileSort.Sorters;

public class ExternalMergeFileSorter<T> : IFileSorter<T> where T : IFileLine
{
    private readonly IMerger<T> _merger;
    private readonly IPreparator<T> _preparator;
    
    public ExternalMergeFileSorter(IMerger<T> merger, IPreparator<T> preparator)
    {
        _merger = merger ?? throw new ArgumentNullException(nameof(merger));
        _preparator = preparator ?? throw new ArgumentNullException(nameof(preparator));
    }
    
    public string SortAndSave(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        
        var folder = Path.GetDirectoryName(path);
        var tempFolder = Path.Combine(folder, "temp");
        var sortedFilePath = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(path)}_sorted.txt");

        // Prepare and sort chunks
        var chunks = _preparator.PrepareChunks(path, tempFolder);
        if(chunks.Count == 0)
        {
            return path;
        }
        
        if (chunks.Count == 1)
        {
            File.Move(chunks.Single(), sortedFilePath);
            return sortedFilePath;
        }

        var finalChunk = _merger.MergeChunks(chunks, tempFolder);
        
        // Rename and move final file
        File.Move(finalChunk, sortedFilePath);

        // Delete temp directory with chunks
        var tempDirectory = new DirectoryInfo(tempFolder);
        tempDirectory.Delete(true);
        
        return sortedFilePath;
    }
}