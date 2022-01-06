using System.Linq;
using Xunit;

namespace YKLang.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("class Foo {}", new[] { "(class Foo)" })]
    [InlineData("class Foo : Bar {}", new[] { "(class Foo : Bar)" })]
    [InlineData("class Foo", new string[] { })]
    public void ClassDeclarationAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("var x = 1;", new[] { "(var x = 1)" })]
    [InlineData("var x = \"Foo\";", new[] { "(var x = Foo)" })]
    [InlineData("var", new string[] { })]
    [InlineData("var x", new string[] { })]
    [InlineData("var x =", new string[] { })]
    public void VarDeclarationAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("for(;;) {}", new[] { "(while True (block ))" })]
    [InlineData("for(var i = 0;;) {}", new[] { "(block (var i = 0)(while True (block )))" })]
    [InlineData("for(; 1 < 10;) {}", new[] { "(while (< 1 10) (block ))" })]
    [InlineData("for(var i = 0; i < 10;) {}", new[] { "(block (var i = 0)(while (< i 10) (block )))" })]
    [InlineData("for(;; i = i + 1) {}", new[] { "(while True (block (block )(; (= i (+ i 1)))))" })]
    [InlineData("for(var i = 0;; i = i + 1) {}", new[] { "(block (var i = 0)(while True (block (block )(; (= i (+ i 1))))))" })]
    [InlineData("for(; 1 < 10; i = i + 1) {}", new[] { "(while (< 1 10) (block (block )(; (= i (+ i 1)))))" })]
    [InlineData("for(var i = 0; i < 10; i = i + 1) {}", new[] { "(block (var i = 0)(while (< i 10) (block (block )(; (= i (+ i 1))))))" })]
    [InlineData("for(var i = 0; i < 10; i = i + 1) { var x = i; }", new[] { "(block (var i = 0)(while (< i 10) (block (block (var x = i))(; (= i (+ i 1))))))" })]
    [InlineData("for", new string[] { })]
    [InlineData("for(;)", new string[] { })]
    [InlineData("for(;;)", new string[] { })]
    [InlineData("for(;;){", new string[] { })]
    [InlineData("for(;;)}", new string[] { })]
    public void ForStatementAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("return 1;", new[] { "(return 1)" })]
    [InlineData("var x = 1; return x;", new[] { "(var x = 1)", "(return x)" })]
    public void ReturnStatementAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("var x = 1 + 2;", new[] { "(var x = (+ 1 2))" })]
    [InlineData("var x = -123 * (45.67);", new[] { "(var x = (* (- 123) (group 45.67)))" })]
    [InlineData("var x = 1 + 2 * 3 - 4;", new[] { "(var x = (- (+ 1 (* 2 3)) 4))" })]
    [InlineData("var x = 1 * 3 - 4 / -2;", new[] { "(var x = (- (* 1 3) (/ 4 (- 2))))" })]
    [InlineData("var x = 0 == 0;", new[] { "(var x = (== 0 0))" })]
    [InlineData("var x = 0 != 0;", new[] { "(var x = (!= 0 0))" })]
    [InlineData("var x = 0 < 0;", new[] { "(var x = (< 0 0))" })]
    [InlineData("var x = 0 <= 0;", new[] { "(var x = (<= 0 0))" })]
    [InlineData("var x = 0 > 0;", new[] { "(var x = (> 0 0))" })]
    [InlineData("var x = 0 >= 0;", new[] { "(var x = (>= 0 0))" })]
    public void ExpressionAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    private static void AssertAst(string source, string[] expected)
    {
        var tokens = Lexer.Analyze(source);
        var statements = Parser.Parse(source, tokens).ToArray();
        Assert.Equal(expected.Length, statements.Length);
        var astBuilder = new AstStringBuilder(source);
        for (var i = 0; i < expected.Length; i++)
        {
            var actual = astBuilder.ToString(statements[i]);
            Assert.Equal(expected[i], actual);
        }
    }
}
