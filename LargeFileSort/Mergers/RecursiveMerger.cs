using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;
using LargeFileSort.Parsers.Abstract;

namespace LargeFileSort.Mergers;

public class RecursiveMerger<T> : IMerger<T> where T : IFileLine
{
    private readonly IFileLineParser<T> _parser;
    
    public RecursiveMerger(IFileLineParser<T> parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }
    
    public string MergeChunks(List<string> chunks, string tempFolder)
    {
        return Merge(chunks, tempFolder, 0, chunks.Count - 1);
    }
    
    private string Merge(IReadOnlyList<string> chunks, string tempFolder, int left, int right)
    {
        if (left >= right)
        {
            return chunks[left];
        }

        var mid = left + (right - left) / 2;
        var leftChunk = Merge(chunks, tempFolder, left, mid);
        var rightChunk = Merge(chunks, tempFolder, mid + 1, right);

        var newChunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid():N}.txt");
        
        var writeStream = File.OpenWrite(newChunkPath);
        var writer = new StreamWriter(writeStream);
        
        var leftChunkStream = File.OpenRead(leftChunk);
        var leftChunkReader = new StreamReader(leftChunkStream);
        
        var rightChunkStream = File.OpenRead(rightChunk);
        var rightChunkReader = new StreamReader(rightChunkStream);

        try
        {
            var leftLine = leftChunkReader.ReadLine();
            var rightLine = rightChunkReader.ReadLine();

            var leftFileLine = _parser.Parse(leftLine);
            var rightFileLine = _parser.Parse(rightLine);
            while (leftFileLine is not null && rightFileLine is not null)
            {
                if (leftFileLine.CompareTo(rightFileLine) <= 0)
                {
                    writer.WriteLine(leftLine);

                    leftLine = leftChunkReader.ReadLine();
                    leftFileLine = _parser.Parse(leftLine);
                }
                else
                {
                    writer.WriteLine(rightLine);

                    rightLine = rightChunkReader.ReadLine();
                    rightFileLine = _parser.Parse(rightLine);
                }
            }

            while (leftFileLine is not null)
            {
                writer.WriteLine(leftLine);

                leftLine = leftChunkReader.ReadLine();
                leftFileLine = _parser.Parse(leftLine);
            }

            while (rightFileLine is not null)
            {
                writer.WriteLine(rightLine);

                rightLine = rightChunkReader.ReadLine();
                rightFileLine = _parser.Parse(rightLine);
            }
        }
        finally
        {
            writer.Dispose();
            leftChunkReader.Dispose();
            rightChunkReader.Dispose();
        }
        
        File.Delete(leftChunk);
        File.Delete(rightChunk);

        return newChunkPath;
    }
}