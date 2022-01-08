using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace YKLang.Tests;

public class LexerTests
{
    [Fact]
    public void SingleCharacterTokensTest()
    {
        const string Source = "(){},.+-*/:;";
        var expected = new ValuedToken[]
        {
            new(TokenType.LeftParen, "("),
            new(TokenType.RightParen, ")"),
            new(TokenType.LeftBrace, "{"),
            new(TokenType.RightBrace, "}"),
            new(TokenType.Comma, ","),
            new(TokenType.Dot, "."),
            new(TokenType.Plus, "+"),
            new(TokenType.Minus, "-"),
            new(TokenType.Multiply, "*"),
            new(TokenType.Divide, "/"),
            new(TokenType.Colon, ":"),
            new(TokenType.Semicolon, ";")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void HashTokenTest()
    {
        const string Source = "# Comment 1\n{}# Comment 2\n";
        var expected = new ValuedToken[]
        {
            new(TokenType.Hash, "# Comment 1\n"),
            new(TokenType.LeftBrace, "{"),
            new(TokenType.RightBrace, "}"),
            new(TokenType.Hash, "# Comment 2\n")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void DoubleCharactersTokensTest()
    {
        const string Source = "! != = == < <= > >= & && | ||";
        var expected = new ValuedToken[]
        {
            new(TokenType.Not, "!"),
            new(TokenType.NotEqual, "!="),
            new(TokenType.Assign, "="),
            new(TokenType.Equal, "=="),
            new(TokenType.Less, "<"),
            new(TokenType.LessEqual, "<="),
            new(TokenType.Greater, ">"),
            new(TokenType.GreaterEqual, ">="),
            new(TokenType.BitwiseAnd, "&"),
            new(TokenType.And, "&&"),
            new(TokenType.BitwiseOr, "|"),
            new(TokenType.Or, "||")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void StringTokenTest()
    {
        const string Source = "{\"This is String\"}";
        var expected = new ValuedToken[]
        {
            new(TokenType.LeftBrace, "{"),
            new(TokenType.String, "This is String"),
            new(TokenType.RightBrace, "}")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void NumberTokenTest()
    {
        const string Source = "{0.5.}";
        var expected = new ValuedToken[]
        {
            new(TokenType.LeftBrace, "{"),
            new(TokenType.Number, "0.5"),
            new(TokenType.Dot, "."),
            new(TokenType.RightBrace, "}")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void BooleanTokenTest()
    {
        const string Source = "true false";
        var expected = new ValuedToken[]
        {
            new(TokenType.True, "true"),
            new(TokenType.False, "false")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);

    }

    [Fact]
    public void KeywordTokenTest()
    {
        const string Source = "class if else while for nil return this base var function foo";
        var expected = new ValuedToken[]
        {
            new(TokenType.Class, "class"),
            new(TokenType.If, "if"),
            new(TokenType.Else, "else"),
            new(TokenType.While, "while"),
            new(TokenType.For, "for"),
            new(TokenType.Nil, "nil"),
            new(TokenType.Return, "return"),
            new(TokenType.This, "this"),
            new(TokenType.Base, "base"),
            new(TokenType.Var, "var"),
            new(TokenType.Function, "function"),
            new(TokenType.Identifier, "foo")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void DefineVariableTest()
    {
        const string Source = "var x = 1.0";
        var expected = new ValuedToken[]
        {
            new(TokenType.Var, "var"),
            new(TokenType.Identifier, "x"),
            new(TokenType.Assign, "="),
            new(TokenType.Number, "1.0"),
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Fact]
    public void DefineFunctionTest()
    {
        const string Source = "function F(x, y) {}";
        var expected = new ValuedToken[]
        {
            new(TokenType.Function, "function"),
            new(TokenType.Identifier, "F"),
            new(TokenType.LeftParen, "("),
            new(TokenType.Identifier, "x"),
            new(TokenType.Comma, ","),
            new(TokenType.Identifier, "y"),
            new(TokenType.RightParen, ")"),
            new(TokenType.LeftBrace, "{"),
            new(TokenType.RightBrace, "}")
        };

        var actual = Lexer.Analyze(Source);
        AssertTokens(expected, actual);
    }

    [Theory]
    [InlineData("Hello", "Hello", true)]
    [InlineData("Hello World", "Hello", true)]
    [InlineData("Hello+", "Hello", true)]
    [InlineData("HelloWorld", "Hello", false)]
    [InlineData("Hello World", "World", false)]
    [InlineData("Hell", "Hello", false)]
    public void IsMatchTest(string source, string target, bool expected)
    {
        var actual = Lexer.IsMatch(source, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("# Comment", 9)]
    [InlineData("# Comment\n", 10)]
    [InlineData("# Comment\n{}", 10)]
    public void CommentLengthTest(string source, int expected)
    {
        var actual = Lexer.CommentLength(source);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("\"\"", 2)]
    [InlineData("\"String\"", 8)]
    [InlineData("\"A\nB\"", 5)]
    [InlineData("\"A", -1)]
    public void StringLengthTest(string source, int expected)
    {
        var actual = Lexer.StringLength(source);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("0", 1)]
    [InlineData("1000", 4)]
    [InlineData("0.5", 3)]
    [InlineData("5.5.5", -1)]
    [InlineData("5 ", 1)]
    public void NumberLengthTest(string source, int expected)
    {
        var actual = Lexer.NumberLength(source);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("class Foo", 5)]
    [InlineData("if ()", 2)]
    [InlineData("foo", 3)]
    public void OtherLengthTest(string source, int expected)
    {
        var actual = Lexer.OtherLength(source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AssertTokensTest()
    {
        const string Source = "( foo )";
        var expected = new ValuedToken[]
        {
            new(TokenType.LeftParen, "("), new(TokenType.Identifier, "foo"), new(TokenType.RightParen, ")")
        };
        var actual = new Token[]
        {
            new(TokenType.LeftParen, 0..1), new(TokenType.Identifier, 2..5), new(TokenType.RightParen, 6..7)
        };

        AssertTokens(expected, new ParsableObject(Source, actual));
    }

    private static void AssertTokens(IEnumerable<ValuedToken> expected, ParsableObject actual)
    {
        Assert.Equal(expected, actual.Tokens.Select(x => new ValuedToken(x.Type, actual.Source[x.Range])));
    }

    private record ValuedToken(TokenType Type, string Value);
}
