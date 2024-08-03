namespace TestFileGeneratorTool;

class Program
{
    static void Main(string[] args)
    {
        var filePath = Console.ReadLine();
        var maxSizeMbInput = Console.ReadLine();
        
        var maxSizeMb = int.Parse(maxSizeMbInput);

        TestFileGenerator.GenerateRandom(filePath, maxSizeMb);
        Console.WriteLine($"File has been created: {filePath}");
    }
}