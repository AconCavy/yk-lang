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
            new(TokenType.LeftParen, 0..1),
            new(TokenType.RightParen, 1..2),
            new(TokenType.LeftBrace, 2..3),
            new(TokenType.RightBrace, 3..4),
            new(TokenType.Comma, 4..5),
            new(TokenType.Dot, 5..6),
            new(TokenType.Plus, 6..7),
            new(TokenType.Minus, 7..8),
            new(TokenType.Star, 8..9),
            new(TokenType.Slash, 9..10),
            new(TokenType.Semicolon, 10..11),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HashTokenTest()
    {
        const string Source = "# Comment 1\n{}# Comment 2\n";
        var expected = new List<Token>
        {
            new(TokenType.Hash, 0..12),
            new(TokenType.LeftBrace, 12..13),
            new(TokenType.RightBrace, 13..14),
            new(TokenType.Hash, 14..26),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DoubleCharactersTokensTest()
    {
        const string Source = "! != = == < <= > >= & && | ||";
        var expected = new List<Token>
        {
            new(TokenType.Bang, 0..1),
            new(TokenType.BangEqual, 2..4),
            new(TokenType.Assign, 5..6),
            new(TokenType.Equal, 7..9),
            new(TokenType.Less, 10..11),
            new(TokenType.LessEqual, 12..14),
            new(TokenType.Greater, 15..16),
            new(TokenType.GreaterEqual, 17..19),
            new(TokenType.BitwiseAnd, 20..21),
            new(TokenType.And, 22..24),
            new(TokenType.BitwiseOr, 25..26),
            new(TokenType.Or, 27..29),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void StringTokenTest()
    {
        const string Source = "{\"This is String\"}";
        var expected = new List<Token>
        {
            new(TokenType.LeftBrace, 0..1),
            new(TokenType.String, 2..16),
            new(TokenType.RightBrace, 17..18),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NumberTokenTest()
    {
        const string Source = "{0.5.}";
        var expected = new List<Token>
        {
            new(TokenType.LeftBrace, 0..1),
            new(TokenType.Number, 1..4),
            new(TokenType.Dot, 4..5),
            new(TokenType.RightBrace, 5..6),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void KeywordTokenTest()
    {
        const string Source = "class foo";
        var expected = new List<Token>
        {
            new(TokenType.Class, 0..5),
            new(TokenType.Identifier, 6..9),
        };

        var actual = Lexer.Analyze(Source);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DefineVariableTest()
    {
        const string Source = "var x = 1.0";
        var expected = new List<Token>
        {
            new(TokenType.Var, 0..3),
            new(TokenType.Identifier, 4..5),
            new(TokenType.Assign, 6..7),
            new(TokenType.Number, 8..11),
        };

        var actual = Lexer.Analyze(Source);
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
}
