using System.Text;
using LargeFileSort.Entities;
using LargeFileSort.Mergers.Abstract;

namespace LargeFileSort.Mergers;

public class TwoWayRecursiveMerger<T> : IMerger<T> where T : IFileLine, new()
{
    public string MergeChunks(List<string> chunks, string tempFolder)
    {
        return Merge(chunks, tempFolder, 0, chunks.Count - 1);
    }
    
    private static string Merge(IReadOnlyList<string> chunks, string tempFolder, int left, int right)
    {
        if (left >= right)
        {
            return chunks[left];
        }

        var mid = left + (right - left) / 2;
        var leftChunk = Merge(chunks, tempFolder, left, mid);
        var rightChunk = Merge(chunks, tempFolder, mid + 1, right);

        var newChunkPath = Path.Combine(tempFolder, $"chunk_{Guid.NewGuid():N}.txt");
        
        var writer = new StreamWriter(newChunkPath, false, Encoding.ASCII);
        var leftChunkReader = new StreamReader(leftChunk, Encoding.ASCII, true);
        var rightChunkReader = new StreamReader(rightChunk, Encoding.ASCII, true);
        try
        {
            var leftLine = leftChunkReader.ReadLine();
            var rightLine = rightChunkReader.ReadLine();
            while (!string.IsNullOrEmpty(leftLine) && !string.IsNullOrEmpty(rightLine))
            {
                var leftFileLine = new T { OriginalLine = leftLine };
                var rightFileLine = new T { OriginalLine = rightLine };
                
                if (leftFileLine.CompareTo(rightFileLine) <= 0)
                {
                    writer.WriteLine(leftLine);
                    leftLine = leftChunkReader.ReadLine();
                }
                else
                {
                    writer.WriteLine(rightLine);
                    rightLine = rightChunkReader.ReadLine();
                }
            }

            while (!string.IsNullOrEmpty(leftLine))
            {
                writer.WriteLine(leftLine);
                leftLine = leftChunkReader.ReadLine();
            }

            while (!string.IsNullOrEmpty(rightLine))
            {
                writer.WriteLine(rightLine);
                rightLine = rightChunkReader.ReadLine();
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