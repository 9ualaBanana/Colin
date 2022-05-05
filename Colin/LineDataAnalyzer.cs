using Colin.LineFilters;

namespace Colin;

/// <summary>
/// Analyzes the line data for files taking into account optional <see cref="ILineFilter"/>s applied to them.
/// </summary>
/// <remarks>
/// Represents an object that can be applied to files and directories containing them for analyzing their line data.
/// Can accept <see cref="ILineFilter"/>s to filter the lines that should be analyzed.
/// </remarks>
internal class LineDataAnalyzer
{
    readonly ILineFilter _lineFilter;

    internal LineDataAnalyzer() : this(new LineFilter())
    {
    }

    internal LineDataAnalyzer(LineFilter lineFilter)
    {
        _lineFilter = lineFilter;
    }

    /// <summary>
    /// Analyzes the line data of each entry inside a directory.
    /// </summary>
    /// <remarks>
    /// Recursively applied to each nested entry. <paramref name="searchOption"/> specifies if <see cref="LineData"/>
    /// of nested directories should be aggregated.
    /// </remarks>
    /// <param name="path">The path to the directory.</param>
    /// <param name="searchOption">
    /// <see cref="SearchOption.TopDirectoryOnly>"/> aggregates <see cref="LineData"/>
    /// of nested directories; <see cref="SearchOption.AllDirectories"/> outputs <see cref="LineData"/> for each entry.
    /// </param>
    /// <returns><see cref="LineData"/> for each entry in the directory.</returns>
    internal IEnumerable<LineData> ApplyToDirectory(string path, SearchOption searchOption = SearchOption.AllDirectories)
    {
        foreach (var entry in Directory.EnumerateFileSystemEntries(path))
        {
            if (Directory.Exists(entry) && searchOption is SearchOption.AllDirectories)
            {
                // Unpacks nested entries and yields them back into this method.
                foreach (var nestedEntry in ApplyToDirectory(entry))
                {
                    yield return nestedEntry;
                }
            }
            else
            {
                // Files and directories that aggregate the results of their entries always end up here.
                yield return ApplyTo(entry);
            }
        }
    }

    /// <summary>
    /// Analyzes the line data of a file or directory.
    /// </summary>
    /// <remarks>
    /// <see cref="LineData"/> of a directory is an aggregated <see cref="LineData"/> of each of its entries.
    /// </remarks>
    /// <param name="path">The path to a file or directory to which <see cref="LineDataAnalyzer"/> should be applied.</param>
    /// <returns><see cref="LineData"/> of a file or directory.</returns>
    /// <exception cref="FileNotFoundException">The file or directory was not found at <paramref name="path"/>.</exception>
    internal LineData ApplyTo(string path)
    {
        if (File.Exists(path))
        {
            return ApplyToFile(path);
        }
        else if (Directory.Exists(path))
        {
            return ApplyToDirectory(path).Aggregate(
                seed: new LineData(path),
                (aggregatedLineData, entryLineData) => 
                {
                    aggregatedLineData.AllLines.AddRange(entryLineData.AllLines);
                    aggregatedLineData.UnfilteredLines.AddRange(entryLineData.UnfilteredLines);
                    return aggregatedLineData;
                }
            );
        }
        else throw new FileNotFoundException("The entry at the specified path was not found.", path);
    }

    LineData ApplyToFile(string path)
    {
        using var file = File.OpenRead(path);
        return ApplyTo(file, path);
    }

    LineData ApplyTo(FileStream file, string path)
    {
        using var sr = new StreamReader(file);
        return ApplyToCore(sr, path);
    }

    LineData ApplyToCore(StreamReader file, string path)
    {
        var lineData = new LineData(path);
        string? line;
        while ((line = file.ReadLine()) is not null)
        {
            lineData.AllLines.Add(line);
            if (_lineFilter.IsPassedBy(line)) lineData.UnfilteredLines.Add(line);
            else lineData.FilteredLines.Add(line);
        }
        return lineData;
    }
}
