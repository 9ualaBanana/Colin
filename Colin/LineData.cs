namespace Colin;

/// <summary>
/// Stores the line data about a file or directory that went through <see cref="LineDataAnalyzer"/>.
/// </summary>
/// <param name="EntryName">The name of a file or directory.</param>
/// <param name="AllLines">All the lines read from the entry.</param>
/// <param name="UnfilteredLines">The lines that passed <see cref="LineFilters.ILineFilter"/>s.</param>
/// <param name="FilteredLines">The lines that didn't pass <see cref="LineFilters.ILineFilter"/>s</param>
internal record LineData(
    string EntryName,
    List<string> AllLines,
    List<string> UnfilteredLines,
    List<string> FilteredLines
    )
{
    internal LineData(string entryName) : this(entryName, new(), new(), new())
    {
    }
}
