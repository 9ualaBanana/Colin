namespace Colin.LineFilters;

internal class CommentLineFilter : ILineFilter
{
    ILineFilter _singleLineCommentLineFilter = new SingleLineCommentLineFilter();
    ILineFilter _multiLineCommentLineFilter = new MultiLineCommentLineFilter();

    public bool IsPassedBy(string line)
    {
        return _multiLineCommentLineFilter.IsPassedBy(line) && _singleLineCommentLineFilter.IsPassedBy(line);
    }
}
