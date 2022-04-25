namespace Colin.LineFilters;

internal class BracketsLineFilter : ILineFilter
{
    public bool IsPassedBy(string line)
    {
        return line != "{" && line != "}";
    }
}
