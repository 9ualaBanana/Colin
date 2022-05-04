using FluentAssertions;
using System;
using System.Linq;
using System.IO;
using Xunit;

namespace Colin.Test;

public class LineCounterTest
{
    const int numberOfFiles = 2;
    const int numberOfLines = 10;

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ApplyTo_File_ReturnsCorrectLineCount(int lineCount)
    {
        var sut = new LineCounter();
        var path = Path.GetTempFileName();
        var lines = new string[lineCount];
        Array.Fill(lines, "a");
        File.WriteAllLines(path, lines);

        var result = sut.ApplyTo(path);

        result.Should().Be(lineCount);
    }

    [Fact]
    public void ApplyTo_Directory_AggregatesNestedLineCount()
    {
        var sut = new LineCounter();
        var directoryPath = CreateTestingSystemEntries(numberOfLines);

        var result = sut.ApplyTo(directoryPath);

        result.Should().Be(numberOfLines * numberOfFiles);
    }

    [Fact]
    public void ApplyTo_NonExistentFileSystemEntry_Throws()
    {
        var sut = new LineCounter();

        sut.Invoking(sut => sut.ApplyTo(string.Empty)).Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void ApplyToDirectory_TopDirectoryOnly_AggregatesNestedLineCount()
    {
        var sut = new LineCounter();

        var path = CreateTestingSystemEntries(numberOfLines);

        var result = sut.ApplyToDirectory(path, SearchOption.TopDirectoryOnly);

        result.First().Should().Be(numberOfLines * numberOfFiles);
    }

    [Fact]
    public void ApplyToDirectory_AllDirectories_DoesNotAggregateNestedLineCount()
    {
        var sut = new LineCounter();
        var path = CreateTestingSystemEntries(numberOfLines);

        var result = sut.ApplyToDirectory(path);

        result.Count().Should().Be(numberOfFiles);
    }

    [Fact]
    public void ApplyToDirectory_NonExistentPath_Throws()
    {
        var sut = new LineCounter();

        sut.Enumerating(sut => sut.ApplyToDirectory(Path.GetTempFileName())).Should().Throw<IOException>();
    }

    [Fact]
    public void ApplyToDirectory_FilePath_Throws()
    {
        var sut = new LineCounter();

        sut.Enumerating(sut => sut.ApplyToDirectory(Path.GetTempFileName())).Should().Throw<IOException>();
    }

    /// <summary>
    /// Creates top directory with two directories nested in each other. Also creates a file on each level except the top one (2 in total).
    /// </summary>
    /// <param name="numberOfLines">The number of lines to insert in files.</param>
    /// <returns>The path to the top directory.</returns>
    static string CreateTestingSystemEntries(int numberOfLines)
    {
        var topDirectoryPath = @".\Top";
        var files = new string[2]
        {
            $@"{topDirectoryPath}\FirstLevel\TestData.txt",
            $@"{topDirectoryPath}\FirstLevel\SecondLevel\TestData.txt"
        };
        Directory.CreateDirectory($@"{topDirectoryPath}\FirstLevel\SecondLevel");

        var newLines = new string[numberOfLines];
        Array.Fill(newLines, "a");
        foreach (var file in files)
        {
            File.WriteAllLines(file, newLines);
        }
        return topDirectoryPath;
    }
}
