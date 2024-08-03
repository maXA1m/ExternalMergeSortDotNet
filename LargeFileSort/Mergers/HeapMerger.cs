using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;
using LargeFileSort.Parsers.Abstract;

namespace LargeFileSort.Mergers;

public class HeapMerger<T> : IMerger<T> where T : IFileLine
{
    private readonly IFileLineParser<T> _parser;
    
    public HeapMerger(IFileLineParser<T> parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public string MergeChunks(List<string> chunks, string tempFolder)
    {
        // Save streams to use and dispose later
        var streams = new List<(FileStream Stream, StreamReader Reader)>();
        try
        {
            // The upper limit on files opened by .NET is governed by the limit imposed on the Win32 API CreateFile, which is 16384
            foreach (var chunkPath in chunks)
            {
                var chunkStream = File.OpenRead(chunkPath);
                var chunkReader = new StreamReader(chunkStream);

                streams.Add((chunkStream, chunkReader));
            }

            // Initialise heap
            var queue = new PriorityQueue<StreamReader, T>();
            foreach (var stream in streams)
            {
                var line = stream.Reader.ReadLine();
                if (!_parser.TryParse(line, out var fileLine))
                {
                    throw new InvalidDataException();
                }

                queue.Enqueue(stream.Reader, fileLine);
            }

            var finalChunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid().ToString()}.txt");
            
            using var writeStream = File.OpenWrite(finalChunkPath);
            using var writer = new StreamWriter(writeStream);

            // Merge k sorted chunks into one
            while (queue.Count > 0)
            {
                var count = queue.Count;
                while (count-- > 0)
                {
                    if (!queue.TryDequeue(out var reader, out var fileLine))
                    {
                        continue;
                    }

                    writer.WriteLine(fileLine.ToString());

                    var line = reader.ReadLine();
                    if (!_parser.TryParse(line, out fileLine))
                    {
                        continue;
                    }

                    queue.Enqueue(reader, fileLine);
                }
            }

            return finalChunkPath;
        }
        finally
        {
            // Dispose chunk readers
            foreach (var stream in streams)
            {
                stream.Reader.Dispose();
            }
        }
    }
}