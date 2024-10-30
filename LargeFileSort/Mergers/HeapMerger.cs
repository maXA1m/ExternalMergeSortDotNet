using System.Text;
using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;

namespace LargeFileSort.Mergers;

public class HeapMerger<T> : IMerger<T> where T : IFileLine, new()
{
    public string MergeChunks(List<string> chunks, string tempFolder)
    {
        // Save streams to use and dispose later
        var readers = new List<(string ChunkName, StreamReader Reader)>();
        try
        {
            // The upper limit on files opened by .NET is governed by the limit imposed on the Win32 API CreateFile, which is 16384
            readers.AddRange(
                chunks.Select(chunkPath => (chunkPath, new StreamReader(chunkPath, Encoding.ASCII, true))));

            // Initialise heap
            var queue = new PriorityQueue<(string ChunkName, StreamReader Reader), T>(readers.Count);
            foreach (var reader in readers)
            {
                var line = reader.Reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    throw new InvalidDataException();
                }

                queue.Enqueue(reader, new T
                {
                    OriginalLine = line
                });
            }

            var finalChunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid().ToString()}.txt");
            
            // Merge k sorted chunks into one
            using var writer = new StreamWriter(finalChunkPath, false, Encoding.ASCII);
            while (queue.Count > 0)
            {
                var count = queue.Count;
                while (count-- > 0)
                {
                    if (!queue.TryDequeue(out var reader, out var fileLine))
                    {
                        continue;
                    }

                    writer.WriteLine(fileLine.OriginalLine);

                    var line = reader.Reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        // Delete merged file
                        try
                        {
                            reader.Reader.Close();
                            File.Delete(reader.ChunkName);
                        }
                        catch
                        {
                            // Ignored to do not fail sorting
                        }

                        continue;
                    }

                    queue.Enqueue(reader, new T
                    {
                        OriginalLine = line
                    });
                }
            }

            return finalChunkPath;
        }
        finally
        {
            // Dispose chunk readers
            foreach (var (_, reader) in readers)
            {
                reader.Dispose();
            }
        }
    }
}