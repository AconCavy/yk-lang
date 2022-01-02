using Xunit;

namespace YKLang.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("1 + 2", "(+ 1 2)")]
    [InlineData("-123 * (45.67)", "(* (- 123) (group 45.67))")]
    [InlineData("1 + 2 * 3 - 4", "(- (+ 1 (* 2 3)) 4)")]
    [InlineData("1 * 3 - 4 / -2", "(- (* 1 3) (/ 4 (- 2)))")]
    public void CalculationAstTest(string source, string expected)
    {
        var tokens = Lexer.Analyze(source);
        var expression = Parser.Parse(source, tokens);

        var astBuilder = new AstStringBuilder(source);
        var actual = astBuilder.ToString(expression);
        Assert.Equal(expected, actual);
    }
}
