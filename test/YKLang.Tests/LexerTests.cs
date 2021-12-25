using System.Collections.Generic;
using Xunit;

namespace YKLang.Tests;

public class LexerTests
{
    [Fact]
    public void SingleCharacterTokensTest()
    {
        const string Source = "(){},.+-*/;";
        var expected = new List<Token>
        {
            new(TokenType.LeftParen, "("),
            new(TokenType.RightParen, ")"),
            new(TokenType.LeftBrace, "{"),
            new(TokenType.RightBrace, "}"),
            new(TokenType.Comma, ","),
            new(TokenType.Dot, "."),
            new(TokenType.Plus, "+"),
            new(TokenType.Minus, "-"),
            new(TokenType.Star, "*"),
            new(TokenType.Slash, "/"),
            new(TokenType.Semicolon, ";"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HashTokenTest()
    {
        const string Source = "# Comment 1\n{}# Comment 2\n";
        var expected = new List<Token>
        {
            new(TokenType.Hash, "# Comment 1\n"),
            new(TokenType.LeftBrace, "{"),
            new(TokenType.RightBrace, "}"),
            new(TokenType.Hash, "# Comment 2\n"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DoubleCharactersTokensTest()
    {
        const string Source = "! != = == < <= > >=";
        var expected = new List<Token>
        {
            new(TokenType.Bang, "!"),
            new(TokenType.BangEqual, "!="),
            new(TokenType.Assign, "="),
            new(TokenType.Equal, "=="),
            new(TokenType.Less, "<"),
            new(TokenType.LessEqual, "<="),
            new(TokenType.Greater, ">"),
            new(TokenType.GreaterEqual, ">="),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void StringTokenTest()
    {
        const string Source = "{\"This is String\"}";
        var expected = new List<Token>
        {
            new(TokenType.LeftBrace, "{"),
            new(TokenType.String, "This is String"),
            new(TokenType.RightBrace, "}"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NumberTokenTest()
    {
        const string Source = "{0.5.}";
        var expected = new List<Token>
        {
            new(TokenType.LeftBrace, "{"),
            new(TokenType.Number, "0.5"),
            new(TokenType.Dot, "."),
            new(TokenType.RightBrace, "}"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void KeywordTokenTest()
    {
        const string Source = "class foo";
        var expected = new List<Token>
        {
            new(TokenType.Class, "class"),
            new(TokenType.Identifier, "foo"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DefineVariableTest()
    {
        const string Source = "var x = 1.0";
        var expected = new List<Token>
        {
            new(TokenType.Var, "var"),
            new(TokenType.Identifier, "x"),
            new(TokenType.Assign, "="),
            new(TokenType.Number, "1.0"),
        };

        var actual = Lexer.GenerateTokens(Source);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("HelloWorld", "Hello", true)]
    [InlineData("HelloWorld", "World", false)]
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
}
