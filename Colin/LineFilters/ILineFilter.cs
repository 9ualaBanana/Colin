namespace Colin.LineFilters;

internal interface ILineFilter
{
    /// <summary>
    /// Determines if <paramref name="line"/> should be passed or filtered out.
    /// </summary>
    /// <param name="line">The line that goes through the filter.</param>
    /// <returns><c>true</c> if line passed the filter; <c>false</c> if it was filtered out.</returns>
    bool IsPassedBy(string line);
}
