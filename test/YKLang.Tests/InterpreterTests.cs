using System;
using Xunit;
using YKLang.Exceptions;
using YKLang.Expressions;
using YKLang.Statements;

namespace YKLang.Tests;

public class InterpreterTests
{
    [Fact]
    public void VisitBlockStatementTests()
    {
    }

    [Fact]
    public void VisitClassStatementTest()
    {
    }

    [Fact]
    public void VisitExpressionStatementTest()
    {
    }

    [Fact]
    public void VisitFunctionStatementTest()
    {
    }

    [Fact]
    public void VisitIfStatementTest()
    {
    }

    [Fact]
    public void VisitReturnStatementTest()
    {
    }

    [Fact]
    public void VisitVariableStatementTest()
    {
    }

    [Fact]
    public void VisitWhileStatementTest()
    {
    }

    [Fact]
    public void VisitAssignExpressionTest()
    {
    }

    [Theory]
    [InlineData(1, TokenType.Plus, 1, 2)]
    [InlineData("a", TokenType.Plus, "b", "ab")]
    [InlineData(1, TokenType.Minus, 1, 0)]
    [InlineData(1, TokenType.Multiply, 2, 2)]
    [InlineData(2, TokenType.Divide, 2, 1)]
    [InlineData(1, TokenType.Less, 2, true)]
    [InlineData(1, TokenType.Less, 1, false)]
    [InlineData(2, TokenType.Less, 1, false)]
    [InlineData(1, TokenType.LessEqual, 1, true)]
    [InlineData(2, TokenType.LessEqual, 1, false)]
    [InlineData(1, TokenType.Greater, 2, false)]
    [InlineData(1, TokenType.Greater, 1, false)]
    [InlineData(2, TokenType.Greater, 1, true)]
    [InlineData(1, TokenType.GreaterEqual, 1, true)]
    [InlineData(2, TokenType.GreaterEqual, 1, true)]
    [InlineData(0, TokenType.Equal, 1, false)]
    [InlineData(1, TokenType.Equal, 1, true)]
    [InlineData(0, TokenType.NotEqual, 1, true)]
    [InlineData(1, TokenType.NotEqual, 1, false)]
    [InlineData(0, TokenType.None, 0, null)]
    public void VisitBinaryExpressionTest(dynamic left, TokenType op, dynamic right, dynamic expected)
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Binary(new Literal(left), new Token(op, default), new Literal(right));
        var actual = sut.Visit(target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("a", TokenType.Minus, 1)]
    [InlineData("a", TokenType.Multiply, 1)]
    [InlineData("a", TokenType.Divide, 1)]
    [InlineData("a", TokenType.Less, 1)]
    [InlineData("a", TokenType.LessEqual, 1)]
    [InlineData("a", TokenType.Greater, 1)]
    [InlineData("a", TokenType.GreaterEqual, 1)]
    public void VisitBinaryExpressionThrowTest(dynamic left, TokenType op, dynamic right)
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Binary(new Literal(left), new Token(op, default), new Literal(right));
        Assert.Throws<InterpretException>(() => sut.Visit(target));
    }

    [Fact]
    public void VisitCallExpressionTest()
    {
    }

    [Fact]
    public void VisitGetExpressionTest()
    {
    }

    [Fact]
    public void VisitGroupingExpressionTest()
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        const int Expected = 1;
        var target = new Grouping(new Literal(Expected));
        var actual = sut.Visit(target);
        Assert.Equal(Expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData("Foo")]
    [InlineData(true)]
    [InlineData(false)]
    public void VisitLiteralExpressionTest(dynamic value)
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Literal(value);
        var actual = sut.Visit(target);
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(false, TokenType.Or, false, false)]
    [InlineData(false, TokenType.Or, true, true)]
    [InlineData(true, TokenType.Or, false, true)]
    [InlineData(true, TokenType.Or, true, true)]
    [InlineData(false, TokenType.And, false, false)]
    [InlineData(false, TokenType.And, true, false)]
    [InlineData(true, TokenType.And, false, false)]
    [InlineData(true, TokenType.And, true, true)]
    public void VisitLogicalExpressionTest(bool left, TokenType op, bool right, bool expected)
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Logical(new Literal(left), new Token(op, default), new Literal(right));
        var actual = sut.Visit(target);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void VisitSetExpressionTest()
    {
    }

    [Fact]
    public void VisitBaseExpressionTest()
    {
    }

    [Fact]
    public void VisitThisExpressionTest()
    {
    }

    [Theory]
    [InlineData(TokenType.Minus, 1, -1)]
    [InlineData(TokenType.Minus, -1, 1)]
    [InlineData(TokenType.Not, true, false)]
    [InlineData(TokenType.Not, false, true)]
    [InlineData(TokenType.None, 0, null)]
    public void VisitUnaryExpressionTest(TokenType op, dynamic right, dynamic expected)
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Unary(new Token(op, default), new Literal(right));
        var actual = sut.Visit(target);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void VisitUnaryExpressionThrowTest()
    {
        var sut = new Interpreter(new InterpretableObject("", Array.Empty<Statement>()));
        var target = new Unary(new Token(TokenType.Minus, default), new Literal(""));
        Assert.Throws<InterpretException>(() => sut.Visit(target));
    }

    [Theory]
    [InlineData(1)]
    [InlineData("Foo")]
    [InlineData(true)]
    [InlineData(false)]
    public void VisitVariableExpressionTest(dynamic expected)
    {
        var sut = new Interpreter(new InterpretableObject(expected.ToString(), Array.Empty<Statement>()));
        var token = new Token(TokenType.Identifier, ..);
        sut.Visit(new Statements.Variable(token, new Literal(expected)));

        var target = new Expressions.Variable(token);
        var actual = sut.Visit(target);
        Assert.Equal(expected, actual);
    }
}
