namespace TestFileGeneratorTool.Extensions;

public static class RandomExtensions
{
    public static IEnumerable<string> NextStrings(this Random random, string allowedChars, int minLength, int maxLength, int count)
    {
        var chars = new char[maxLength];
        var setLength = allowedChars.Length;
        while (count-- > 0)
        {
            var stringLength = random.Next(minLength, maxLength + 1);
            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[random.Next(setLength)];
            }

            yield return new string(chars, 0, stringLength);
        }
    }

}