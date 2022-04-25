using Colin.LineFilters;
using FluentAssertions;
using Xunit;

namespace Colin.Test;

public class LineFilterTest
{
    [Fact]
    public void NoFilters_IsPassedBy_ReturnsTrue()
    {
        var lineFilter = new LineFilter();
        
        lineFilter.IsPassedBy(string.Empty).Should().BeTrue();
    }

    [Theory]
    [InlineData("//")]
    [InlineData("// comment")]
    [InlineData("//comment")]
    [InlineData(" //comment")]
    [InlineData("#")]
    [InlineData("# comment")]
    [InlineData("   # comment")]
    public void IsPassedBy_SingleLineComment_ReturnsFalse(string singleLineComment)
    {
        var lineFilter = new LineFilter
        {
            new SingleLineCommentLineFilter()
        };

        lineFilter.IsPassedBy(singleLineComment).Should().BeFalse();
    }
    
    [Theory]
    [InlineData("/*\n*/")]
    [InlineData("/*comment\nsecond line*/")]
    [InlineData("/*comment\nsecond line\n*/")]
    [InlineData("/*\ncomment*//*another one \n* */")]
    [InlineData("/*\ncomment*/ /*another one \n* */")]
    public void IsPassedBy_MultiLineComment_ReturnsFalse(string multilineComment)
    {
        var lineFilter = new LineFilter
        {
            new MultiLineCommentLineFilter()
        };

        lineFilter.IsPassedBy(multilineComment).Should().BeFalse();
    }

    [Theory]
    [InlineData("{")]
    [InlineData("not a comment /*comment on the same line*/")]
    [InlineData("/*\ncomment*/ not a comment /*comment on the same line*/")]
    [InlineData("/*comment*/ not a comment")]
    [InlineData("/*comment*/ not a comment/*\n*/")]
    [InlineData("/*comment*/ not a comment/*\ncomment on another line*/")]
    [InlineData("/*comment*/ not a comment /*comment\n on another line\n*/")]
    public void IsPassedBy_MultilineCommentAndNonComment_ReturnsTrue(string commentAndNonComment)
    {
        var lineFilter = new LineFilter
        {
            new MultiLineCommentLineFilter()
        };

        lineFilter.IsPassedBy(commentAndNonComment).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void IsPassedBy_EmptyLine_ReturnsFalse(string emptyLine)
    {
        var lineFilter = new LineFilter
        {
            new EmptyLineFilter()
        };

        lineFilter.IsPassedBy(emptyLine).Should().BeFalse();
    }

    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("{  ")]
    [InlineData("}  ")]
    [InlineData("   }  ")]
    [InlineData("   {  ")]
    public void IsPassedBy_BracketsOnlyLine_ReturnsFalse(string bracketOnlyLine)
    {
        var lineFilter = new LineFilter
        {
            new BracketsLineFilter()
        };

        lineFilter.IsPassedBy(bracketOnlyLine).Should().BeFalse();
    }
}
