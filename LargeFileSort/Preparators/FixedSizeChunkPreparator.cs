using LargeFileSort.Entities;
using LargeFileSort.Parsers.Abstract;
using LargeFileSort.Preparators.Abstract;

namespace LargeFileSort.Preparators;

public class FixedSizeChunkPreparator<T> : IPreparator<T> where T : IFileLine
{
    private readonly long _maxChunkSize;
    private readonly IFileLineParser<T> _parser;

    private const double MaxMemoryUsageAllowed = 0.2;
    
    public FixedSizeChunkPreparator(IFileLineParser<T> parser, long? chunkSizeBytes = null)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        
        var memoryInfo = GC.GetGCMemoryInfo();
        var totalMemoryBytes = memoryInfo.TotalAvailableMemoryBytes;

        _maxChunkSize = (long) (totalMemoryBytes * MaxMemoryUsageAllowed);

        if (chunkSizeBytes is > 100)
        {
            _maxChunkSize = Math.Min(_maxChunkSize, chunkSizeBytes.Value);
        }
    }
    
    public List<string> PrepareChunks(string path, string tempFolder)
    {
        Directory.CreateDirectory(tempFolder);
        
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);

        var items = new List<T>();
        var chunks = new List<string>();

        var windowBytes = 0L;
        var line = reader.ReadLine();
        while (_parser.TryParse(line, out var fileLine))
        {
            items.Add(fileLine);
            
            windowBytes += System.Text.Encoding.ASCII.GetByteCount(line);
            if (windowBytes >= _maxChunkSize)
            {
                var chunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid():N}.txt");
                SortAndSaveChunk(chunkPath, items);
                    
                chunks.Add(chunkPath);
                items.Clear();
                
                windowBytes = 0;
            }

            line = reader.ReadLine();
        }

        if (string.IsNullOrEmpty(line) && items.Count > 0)
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
        // Assume it is an inplace 3-way QuickSort
        items.Sort();
        
        using var writeStream = File.OpenWrite(chunkPath);
        using var writer = new StreamWriter(writeStream);

        foreach (var item in items)
        {
            writer.WriteLine(item.ToString());
        }
    }
}