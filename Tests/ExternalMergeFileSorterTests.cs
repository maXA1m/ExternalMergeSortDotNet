using LargeFileSort.Entities;
using LargeFileSort.Mergers;
using LargeFileSort.Parsers;
using LargeFileSort.Preparators;
using LargeFileSort.Sorters;
using TestFileGeneratorTool;

namespace Tests;

public class ExternalMergeFileSorterTests
{
    [Test]
    public void ExternalMergeForExampleFromTask()
    {
        // Arrange
        var lines = new List<string>
        {
            "415. Apple",
            "30432. Something something something",
            "1. Apple",
            "32. Cherry is the best",
            "2. Banana is yellow"
        };
        
        var expectedLines = new List<string>
        {
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        };
        
        var parser = new KeyValueFileLineParser();
        var merger = new RecursiveMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var tempPath = Path.Combine(folder, "temp");
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        var expectedFilePath = Path.Combine(folder, "expected.txt");
        
        TestFileGenerator.GenerateFromList(filePath, lines);
        TestFileGenerator.GenerateFromList(expectedFilePath, expectedLines);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(File.ReadAllText(result), Is.EqualTo(File.ReadAllText(expectedFilePath)));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
        File.Delete(expectedFilePath);
        Directory.Delete(tempPath, true);
    }
    
    [Test]
    public void ExternalMergeForExampleFromTaskWithDuplicates()
    {
        // Arrange
        var lines = new List<string>
        {
            "415. Apple",
            "416. Apple",
            "414. Apple",
            "30432. Something something something",
            "1. Apple",
            "32. Cherry is the best",
            "2. Banana is yellow",
            "0. Banana is yellow",
            "1. Banana is yellow"
        };
        
        var expectedLines = new List<string>
        {
            "1. Apple",
            "414. Apple",
            "415. Apple",
            "416. Apple",
            "0. Banana is yellow",
            "1. Banana is yellow",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        };
        
        var parser = new KeyValueFileLineParser();
        var merger = new RecursiveMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var tempPath = Path.Combine(folder, "temp");
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        var expectedFilePath = Path.Combine(folder, "expected.txt");
        
        TestFileGenerator.GenerateFromList(filePath, lines);
        TestFileGenerator.GenerateFromList(expectedFilePath, expectedLines);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(File.ReadAllText(result), Is.EqualTo(File.ReadAllText(expectedFilePath)));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
        File.Delete(expectedFilePath);
        Directory.Delete(tempPath, true);
    }
    
    [Test]
    public void GenerateAndSort1GbOfData()
    {
        // Arrange
        var parser = new KeyValueFileLineParser();
        var merger = new RecursiveMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, 1000);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    [Test]
    public void GenerateAndSort1GbOfDataWithHeapMerger()
    {
        // Arrange
        var parser = new KeyValueFileLineParser();
        var merger = new HeapMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, 1000);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    [Test]
    public void GenerateAndSort1GbOfDataWith100MbChunkSize()
    {
        // Arrange
        var parser = new KeyValueFileLineParser();
        var merger = new RecursiveMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser, 100 * 1000000);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, 1000);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    [Test]
    public void GenerateAndSort1GbOfDataWithHeapMergerAnd100MbChunkSize()
    {
        // Arrange
        var parser = new KeyValueFileLineParser();
        var merger = new HeapMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser, 100 * 1000000);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, 1000);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    //[Test]
    public void GenerateAndSort1GbOfDataWithHeapMergerMoreThan16384Chunks_ShouldFail()
    {
        // Arrange
        var parser = new KeyValueFileLineParser();
        var merger = new HeapMerger<KeyValueFileLine>(parser);
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(parser, 50000);
        
        var sorter = new ExternalMergeFileSorter<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var tempPath = Path.Combine(folder, "temp");
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, 1000);
        
        // Assert
        Assert.Throws<Exception>(() => sorter.SortAndSave(filePath));
        
        // Cleanup
        File.Delete(filePath);
        Directory.Delete(tempPath, true);
    }
}