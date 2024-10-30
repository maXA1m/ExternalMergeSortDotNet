using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;
using LargeFileSort.Preparators.Abstract;
using LargeFileSort.Sorts.Abstract;

namespace LargeFileSort.Sorts;

public class ExternalMergeFileSort<T>(IMerger<T> merger, IPreparator<T> preparator) : IFileSort<T>
    where T : IFileLine, new()
{
    private readonly IMerger<T> _merger = merger ?? throw new ArgumentNullException(nameof(merger));
    private readonly IPreparator<T> _preparator = preparator ?? throw new ArgumentNullException(nameof(preparator));

    public string SortAndSave(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var folder = Path.GetDirectoryName(path);
        var tempFolder = Path.Combine(folder, "temp");
        var sortedFilePath = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(path)}_sorted.txt");

        // Prepare and sort chunks
        var chunks = _preparator.PrepareChunks(path, tempFolder);
        if (chunks.Count == 0)
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
        Directory.Delete(tempFolder, true);

        return sortedFilePath;
    }
}