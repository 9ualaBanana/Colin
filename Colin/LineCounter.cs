using Colin.LineFilters;

namespace Colin;

/// <summary>
/// Counts the number of lines in files taking into account optional <see cref="ILineFilter"/>s applied to them.
/// </summary>
/// <remarks>
/// Can accept <see cref="ILineFilter"/>s to filter the lines that should be counted.
/// </remarks>
internal class LineCounter
{
    readonly ILineFilter _lineFilter;

    internal LineCounter() : this(new LineFilter())
    {
    }

    internal LineCounter(LineFilter lineFilter)
    {
        _lineFilter = lineFilter;
    }

    /// <summary>
    /// Counts the number of lines in a system entry located at the provided <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to a file/directory to which it should be applied.</param>
    /// <returns>The combined number of lines inside a system entry.</returns>
    /// <exception cref="FileNotFoundException">File or directory was not found at <paramref name="path"/>.</exception>
    internal int ApplyTo(string path)
    {
        if (File.Exists(path))
        {
            return ApplyToFile(path);
        }
        else if (Directory.Exists(path))
        {
            return ApplyToDirectory(path, SearchOption.TopDirectoryOnly).Aggregate(
                seed: 0,
                (totalLineCount, entryLineCount) => totalLineCount += entryLineCount
                );
        }
        else throw new FileNotFoundException("The entry with the specified path was not found.", path);
    }

    /// <summary>
    /// Counts the number of lines in files for each entry inside a directory located at <paramref name="path"/>.
    /// </summary>
    /// <remarks>
    /// Recursively applied to each nested entry. <paramref name="searchOption"/> specifies whether the line count
    /// of nested directories should be aggregated into a single value.<br/>
    /// <see cref="SearchOption.TopDirectoryOnly"/> - aggregates; <see cref="SearchOption.AllDirectories"/> does not.
    /// </remarks>
    /// <param name="path">The path to the directory.</param>
    /// <param name="searchOption"/><see cref="SearchOption.TopDirectoryOnly>"/> aggregates line count of nested directories into a single value.
    /// <returns>The number of lines for each entry in the directory.</returns>
    internal IEnumerable<int> ApplyToDirectory(string path, SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(path))
        {
            var exceptionMessage = File.Exists(path) ?
                $"{nameof(path)} must refer to a directory and not a file." :
                $"{nameof(path)} does not exist.";

            throw new DirectoryNotFoundException(exceptionMessage);
        }
        foreach (var entry in Directory.EnumerateFileSystemEntries(path))
        {
            if (File.Exists(entry))
            {
                yield return ApplyToFile(entry);
                continue;
            }

            if (searchOption is SearchOption.TopDirectoryOnly)
            {
                yield return ApplyToDirectory(entry).Aggregate(
                    seed: 0,
                    (totalLineCount, entryLineCount) => totalLineCount += entryLineCount
                    );
            }
            else
            {
                foreach (var nestedDirectory in ApplyToDirectory(entry))
                {
                    yield return nestedDirectory;
                }
            }
        }
    }

    int ApplyToFile(string path)
    {
        using var file = File.OpenRead(path);
        return ApplyTo(file);
    }

    int ApplyTo(FileStream file)
    {
        using var sr = new StreamReader(file);
        return ApplyTo(sr);
    }

    int ApplyTo(StreamReader file)
    {
        var lineCount = 0;
        string? line;
        while ((line = file.ReadLine()) is not null)
        {
            if (_lineFilter.IsPassedBy(line)) lineCount++;
        }
        return lineCount;
    }
}
