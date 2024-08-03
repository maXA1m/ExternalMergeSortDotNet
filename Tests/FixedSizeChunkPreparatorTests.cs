using LargeFileSort.Entities;
using LargeFileSort.Parsers;
using LargeFileSort.Preparators;
using TestFileGeneratorTool;

namespace Tests;

public class FixedSizeChunkPreparatorTests
{
    [Test]
    public void PreparatorCanHandleFileWithDuplicatesLargerThanChunk()
    {
        // Arrange
        const int chunkSizeBytes = 100000;
        const string line = "12312312312. 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";
        
        var parser = new KeyValueFileLineParser();
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser, chunkSizeBytes);
        
        var folder = Directory.GetCurrentDirectory();
        var tempPath = Path.Combine(folder, "temp");
        var filePath = Path.Combine(folder, $"duplicates_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateFromLine(filePath, chunkSizeBytes * 100, line);
        
        // Assert
        Assert.DoesNotThrow(() => preparator.PrepareChunks(filePath, tempPath));
        Assert.That(new FileInfo(filePath).Length,
            Is.EqualTo(Directory.GetFiles(tempPath).Sum(x => new FileInfo(x).Length)));
        
        // Cleanup
        File.Delete(filePath);
        Directory.Delete(tempPath, true);
    }
}