using System.Collections;

namespace Colin.LineFilters;

/// <summary>
/// Safe interface for running a collection of <see cref="ILineFilter"/> on lines.
/// </summary>
internal class LineFilter : ILineFilter, ICollection<ILineFilter>
{

    readonly List<ILineFilter> _linesFilters = new();

    /// <summary>
    /// Incorporates <see cref="EmptyLineFilter"/> and <see cref="CommentLineFilter"/>.
    /// </summary>
    internal static LineFilter Default => new()
    {
        new EmptyLineFilter(),
        new CommentLineFilter()
    };

    public bool IsPassedBy(string line)
    {
        if (line is null) throw new ArgumentNullException(nameof(line));

        line = line.Trim();
        bool passed = true;
        foreach (var linesFilter in _linesFilters)
        {
            if (!linesFilter.IsPassedBy(line))
            {
                passed = false;
            }
        }
        return passed;
    }

    #region ICollection
    public int Count => _linesFilters.Count;

    public bool IsReadOnly => false;

    public void Add(ILineFilter linesFilter)
    {
        _linesFilters.Add(linesFilter);
    }

    public void Clear()
    {
        _linesFilters.Clear();
    }

    public bool Contains(ILineFilter linesFilter)
    {
        return _linesFilters.Contains(linesFilter);
    }

    public void CopyTo(ILineFilter[] array, int arrayIndex)
    {
        _linesFilters.CopyTo(array, arrayIndex);
    }

    public IEnumerator<ILineFilter> GetEnumerator()
    {
        return _linesFilters.GetEnumerator();
    }

    public bool Remove(ILineFilter linesFilter)
    {
        return _linesFilters.Remove(linesFilter);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}
