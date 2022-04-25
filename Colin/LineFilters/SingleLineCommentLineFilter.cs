namespace Colin.LineFilters;

internal class SingleLineCommentLineFilter : ILineFilter
{
    public bool IsPassedBy(string line)
    {
        return !line.StartsWith("//") && !line.StartsWith('#');
    }
}
