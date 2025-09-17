using ExampleLib.UnitTests.Helpers;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;
using System.Text;

namespace ExampleLib.UnitTests;

public class FileUtilTests
{

    public static TheoryData<string[], string[]> LineNumberingData()
    {
        return new TheoryData<string[], string[]>
        {
            { new string[] { "Hello", "World" }, new string[] { "1. Hello", "2. World" } },
            { new string[] { "Line1", "", "Line3" }, new string[] { "1. Line1", "2.", "3. Line3" } },
            { new string[] { "Привет", "😊 Emoji", "中文行" }, new string[] { "1. Привет", "2. 😊 Emoji", "3. 中文行" } },
        };
    }

    // ------------------------
    // Parameterized test
    // ------------------------
    [Theory]
    [MemberData(nameof(LineNumberingData))]
    public void AddLineNumbers_CanAddLineNumbersCorrectly(string[] inputLines, string[] expectedLines)
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, inputLines, Encoding.UTF8);

        // Act
        FileUtil.AddLineNumbers(tempFile);

        // Assert
        string[] actualLines = File.ReadAllLines(tempFile, Encoding.UTF8);
        Assert.Equal(expectedLines.Length, actualLines.Length);
        for (int i = 0; i < expectedLines.Length; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }

    [Fact]
    public void CanAddLineNumbers()
    {
        const string withoutNumbers = """
Hello, world!
This is a test file.
It has empty lines above and below.
1234567890
Special chars: !@#$%^&*()

Привет, мир!
""";
        const string withNumbers = """
1. Hello, world!
2. This is a test file.
3. It has empty lines above and below.
4. 1234567890
5. Special chars: !@#$%^&*()
6.
7. Привет, мир!
""";
        using TempFile file = TempFile.Create(withoutNumbers);
        FileUtil.AddLineNumbers(file.Path);

        string result = File.ReadAllText(file.Path);
        Assert.Equal(withNumbers, result);
    }

    [Fact]
    public void CanSortTextFile()
    {
        const string unsorted = """
                                Играют волны — ветер свищет,
                                И мачта гнется и скрыпит…
                                Увы! он счастия не ищет
                                И не от счастия бежит!
                                """;
        const string sorted = """
                              И мачта гнется и скрыпит…
                              И не от счастия бежит!
                              Играют волны — ветер свищет,
                              Увы! он счастия не ищет
                              """;

        using TempFile file = TempFile.Create(unsorted);
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal(sorted.Replace("\r\n", "\n"), actual);
    }

    [Fact]
    public void CanSortOneLineFile()
    {
        using TempFile file = TempFile.Create("Играют волны — ветер свищет,");
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("Играют волны — ветер свищет,", actual);
    }

    [Fact]
    public void CanSortEmptyFile()
    {
        using TempFile file = TempFile.Create("");

        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }
}