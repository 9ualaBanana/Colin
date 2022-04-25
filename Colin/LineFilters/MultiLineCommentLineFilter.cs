namespace Colin.LineFilters;

internal class MultiLineCommentLineFilter : ILineFilter
{
    const int _bracketLength = 2;
    const int _bracketOffset = _bracketLength - 1;
    /// <summary>
    /// Keeps the state of a multiline comment.
    /// </summary>
    /// <remarks>
    /// Multiline comment can start and end either on the same or different lines. This flag is switched
    /// each time a multiline comment brackets are being opened or closed.
    /// </remarks>
    bool _isMultiLineComment;
    string _line = null!;

    public bool IsPassedBy(string line)
    {
        _line = line;
        // In case the line is too short for the algorithm.
        bool hasUncommentedText = _line.Length < _bracketLength && IsUncommented(_line.Length - 1);

        for (int i = 0; i < line.Length - _bracketOffset; i++)
        {
            if (IsCommentStart(i) || IsCommentEnd(i))
            {
                _isMultiLineComment = !_isMultiLineComment;
                i += _bracketOffset;
                continue;
            }

            if (IsUncommented(i)) hasUncommentedText = true;
        }

        return hasUncommentedText;
    }

    /// <summary>
    /// Determines if the character at <paramref name="charIndex"/> is the part of the multiline comment.
    /// </summary>
    /// <remarks>
    /// To check if an empty line is part of the multiline comment, <paramref name="charIndex"/> needs to be -1.
    /// </remarks>
    /// <param name="charIndex">The index of a character to be checked, -1 if the line is empty.</param>
    /// <returns><c>true</c> if <paramref name="charIndex"/> is part of of the multiline comment; <c>false</c> otherwise.</returns>
    bool IsUncommented(int charIndex)
    {
        // The line is empty.
        if (charIndex < 0) return !_isMultiLineComment;
        return !_isMultiLineComment && !char.IsWhiteSpace(_line[charIndex]);
    }

    bool IsCommentStart(int bracketStartIndex)
    {
        return IsOpeningBracket(bracketStartIndex) && !_isMultiLineComment;
    }

    bool IsCommentEnd(int bracketStartIndex)
    {
        return IsClosingBracket(bracketStartIndex) && _isMultiLineComment;
    }

    bool IsOpeningBracket(int bracketStartIndex)
    {
        return _line[bracketStartIndex] == '/' && _line[bracketStartIndex + 1] == '*';
    }

    bool IsClosingBracket(int bracketStartIndex)
    {
        return _line[bracketStartIndex] == '*' && _line[bracketStartIndex + 1] == '/';
    }
}
