using System.Text;
using LargeFileSort.Entities;
using LargeFileSort.Preparators.Abstract;

namespace LargeFileSort.Preparators;

public class FixedSizeChunkPreparator<T> : IPreparator<T> where T : IFileLine, new()
{
    private readonly long _maxChunkSizeBytes;
    private readonly long _averageLineSizeBytes;

    // Makes chunk size smaller than actual RAM
    // It is recommended to set higher values in real scenarios
    // In practise, however, C# works better with smaller chunks
    private const double MaxMemoryUsageAllowed = 0.2;
    
    public FixedSizeChunkPreparator(int averageLineSizeBytes, long? chunkSizeBytes = null)
    {
        var memoryInfo = GC.GetGCMemoryInfo();
        var totalMemoryBytes = memoryInfo.TotalAvailableMemoryBytes;

        _averageLineSizeBytes = averageLineSizeBytes;
        _maxChunkSizeBytes = (long) (totalMemoryBytes * MaxMemoryUsageAllowed);

        if (chunkSizeBytes is > 100)
        {
            _maxChunkSizeBytes = Math.Min(_maxChunkSizeBytes, chunkSizeBytes.Value);
        }
    }
    
    public List<string> PrepareChunks(string path, string tempFolder)
    {
        Directory.CreateDirectory(tempFolder);
        
        // Allocate only what that file needs
        var fileSizeBytes = new FileInfo(path).Length;
        var chunkItemsCount = Math.Min(_maxChunkSizeBytes, fileSizeBytes) / _averageLineSizeBytes;

        var items = new List<T>((int)Math.Min(chunkItemsCount, int.MaxValue - 1));
        var chunks = new List<string>();

        using var reader = new StreamReader(path, Encoding.ASCII, true);
        
        var windowBytes = 0L;
        var line = reader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            items.Add(new T
            {
                OriginalLine = line
            });

            windowBytes += Encoding.ASCII.GetByteCount(line) + Environment.NewLine.Length;
            
            // Avoid list resize
            if (windowBytes >= _maxChunkSizeBytes || items.Count == items.Capacity)
            {
                var chunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid():N}.txt");
                SortAndSaveChunk(chunkPath, items);
                    
                chunks.Add(chunkPath);
                items.Clear();
                
                windowBytes = 0;
            }

            line = reader.ReadLine();
        }

        if (items.Count > 0)
        {
            var chunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid():N}.txt");
            
            SortAndSaveChunk(chunkPath, items);
                    
            chunks.Add(chunkPath);
            items.Clear();
        }

        return chunks;
    }

    private static void SortAndSaveChunk(string chunkPath, List<T> items)
    {
        // Assume it is 3-way QuickSort
        items.Sort();
        
        using var writer = new StreamWriter(chunkPath, false, Encoding.ASCII);
        
        foreach (var item in items)
        {
            writer.WriteLine(item.OriginalLine);
        }
    }
}