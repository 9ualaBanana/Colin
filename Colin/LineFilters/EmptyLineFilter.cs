namespace Colin.LineFilters;

internal class EmptyLineFilter : ILineFilter
{
    public bool IsPassedBy(string line)
    {
        return !string.IsNullOrWhiteSpace(line);
    }
}
