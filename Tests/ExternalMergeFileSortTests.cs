using LargeFileSort.Entities;
using LargeFileSort.Mergers;
using LargeFileSort.Preparators;
using LargeFileSort.Sorts;
using TestFileGeneratorTool;

namespace Tests;

public class ExternalMergeFileSortTests
{
    [Test]
    public void TwoWayRecursiveMerger_SimpleExampleFromTask()
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
        
        var merger = new TwoWayRecursiveMerger<KeyValueFileLine>();
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(KeyValueFileLine.AverageSizeBytes);
        
        var sorter = new ExternalMergeFileSort<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
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
    }
    
    [Test]
    public void TwoWayRecursiveMerger_SimpleExampleFromTask_WithDuplicates()
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
        
        var merger = new TwoWayRecursiveMerger<KeyValueFileLine>();
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(KeyValueFileLine.AverageSizeBytes);
        
        var sorter = new ExternalMergeFileSort<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
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
    }
    
    [Test]
    [TestCase(1000, null)]
    [TestCase(1000, 100 * 1000000)]
    [TestCase(10000, null)]
    [TestCase(10000, 100 * 1000000)]
    [TestCase(10000, 500 * 1000000)]
    [TestCase(10000, 1000 * 1000000)]
    public void TwoWayRecursiveMerger_GenerateAndSortData(int unsortedFileSizeMb, long? chunkSizeBytes)
    {
        // Arrange
        var merger = new TwoWayRecursiveMerger<KeyValueFileLine>();
        var preparator =
            new FixedSizeChunkPreparator<KeyValueFileLine>(KeyValueFileLine.AverageSizeBytes, chunkSizeBytes);
        
        var sorter = new ExternalMergeFileSort<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, unsortedFileSizeMb);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    [Test]
    [TestCase(1000, null)]
    [TestCase(1000, 100 * 1000000)]
    [TestCase(10000, null)]
    [TestCase(10000, 100 * 1000000)]
    [TestCase(10000, 500 * 1000000)]
    [TestCase(10000, 1000 * 1000000)]
    public void HeapMerger_GenerateAndSortData(int unsortedFileSizeMb, long? chunkSizeBytes)
    {
        // Arrange
        var merger = new HeapMerger<KeyValueFileLine>();
        var preparator =
            new FixedSizeChunkPreparator<KeyValueFileLine>(KeyValueFileLine.AverageSizeBytes, chunkSizeBytes);
        
        var sorter = new ExternalMergeFileSort<KeyValueFileLine>(merger, preparator);
        
        var folder = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(folder, $"unsorted_{Guid.NewGuid():N}.txt");
        
        TestFileGenerator.GenerateRandom(filePath, unsortedFileSizeMb);
        
        // Act
        var result = sorter.SortAndSave(filePath);
        
        // Assert
        Assert.That(new FileInfo(filePath).Length, Is.EqualTo(new FileInfo(result).Length));
        
        // Cleanup
        File.Delete(result);
        File.Delete(filePath);
    }
    
    //[Test]
    public void HeapMerger_GenerateAndSort1GbOfData_WithMoreThan16384Chunks_ShouldFail()
    {
        // Arrange
        var merger = new HeapMerger<KeyValueFileLine>();
        var preparator = new FixedSizeChunkPreparator<KeyValueFileLine>(KeyValueFileLine.AverageSizeBytes, 50000);
        
        var sorter = new ExternalMergeFileSort<KeyValueFileLine>(merger, preparator);
        
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