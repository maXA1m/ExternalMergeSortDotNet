using System.Text;
using TestFileGeneratorTool.Extensions;

namespace TestFileGeneratorTool;

public static class TestFileGenerator
{
    private const int MaxStringLength = 2048;
    private const long BytesInMegabyte = 1000000; // 1048576.0
    private const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
    public static void GenerateRandom(string filePath, int maxSizeMb)
    {
        var maxSizeBytes = maxSizeMb * BytesInMegabyte;
        
        const int stringsBatchSize = 5000;

        var stringRandom = new Random();
        var numberRandom = new Random();

        using var stream = File.OpenWrite(filePath);
        using var writer = new StreamWriter(stream);
        
        var totalBytes = 0L;
        while (totalBytes < maxSizeBytes)
        {
            foreach (var randomString in stringRandom.NextStrings(AllowedChars, 15, MaxStringLength, stringsBatchSize))
            {
                var line = $"{numberRandom.NextInt64()}. {randomString}";
                
                totalBytes += Encoding.ASCII.GetByteCount(line);
                if (totalBytes > maxSizeBytes)
                {
                    return;
                }

                writer.WriteLine(line);
            }
        }
    }
    
    public static void GenerateFromLine(string filePath, long sizeBytes, string line)
    {
        using var stream = File.OpenWrite(filePath);
        using var writer = new StreamWriter(stream);
        
        var lineSize = Encoding.ASCII.GetByteCount(line);
        while (sizeBytes > 0)
        {
            writer.WriteLine(line);
            sizeBytes -= lineSize;
        }
    }
    
    public static void GenerateFromList(string filePath, List<string> lines)
    {
        using var stream = File.OpenWrite(filePath);
        using var writer = new StreamWriter(stream);

        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }
    }
}