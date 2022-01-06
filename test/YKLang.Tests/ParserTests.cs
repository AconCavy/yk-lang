using System.Linq;
using Xunit;

namespace YKLang.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("class Foo {}", new[] { "(class Foo)" })]
    [InlineData("class Foo : Bar {}", new[] { "(class Foo : Bar)" })]
    [InlineData("class Foo { function F() {} }", new[] { "(class Foo (function F()))" })]
    [InlineData("class Foo { function F(a) {} }", new[] { "(class Foo (function F(a)))" })]
    [InlineData("class Foo { function F(a, b) {} }", new[] { "(class Foo (function F(a b)))" })]
    [InlineData("class Foo { function F(a) { return a; } }", new[] { "(class Foo (function F(a) (return a)))" })]
    [InlineData("class Foo { function F() { return this.a; } }",
        new[] { "(class Foo (function F() (return (. this a))))" })]
    [InlineData("class Foo : Bar { function F() { return base.a; } }",
        new[] { "(class Foo : Bar (function F() (return (base a))))" })]
    [InlineData("class Foo", new string[] { })]
    [InlineData("class Foo {", new string[] { })]
    [InlineData("class Foo }", new string[] { })]
    public void ClassDeclarationAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("function F() { }", new[] { "(function F())" })]
    [InlineData("function F() { var x = 1; }", new[] { "(function F() (var x = 1))" })]
    [InlineData("function F(a) { }", new[] { "(function F(a))" })]
    [InlineData("function F(a) { var x = 1; }", new[] { "(function F(a) (var x = 1))" })]
    [InlineData("function F(a, b) { }", new[] { "(function F(a b))" })]
    [InlineData("function F(a, b) { var x = 1; }", new[] { "(function F(a b) (var x = 1))" })]
    public void FunctionDeclarationAstTest(string source, string[] expected)
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
    [InlineData("x = 1;", new[] { "(; (= x 1))" })]
    [InlineData("x = \"Foo\";", new[] { "(; (= x Foo))" })]
    [InlineData("x = true;", new[] { "(; (= x True))" })]
    [InlineData("x = false;", new[] { "(; (= x False))" })]
    [InlineData("x = true || false;", new[] { "(; (= x (|| True False)))" })]
    [InlineData("x = true && false;", new[] { "(; (= x (&& True False)))" })]
    [InlineData("x = nil;", new[] { "(; (= x nil))" })]
    [InlineData("x = (1 + 1);", new[] { "(; (= x (group (+ 1 1))))" })]
    [InlineData("x = F(1);", new[] { "(; (= x (call F 1)))" })]
    [InlineData("x.y = 1;", new[] { "(; (= x y 1))" })]
    [InlineData("1 = 1;", new string[] { })]
    public void AssignmentAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("for(;;) {}", new[] { "(while True (block ))" })]
    [InlineData("for(var i = 0;;) {}", new[] { "(block (var i = 0)(while True (block )))" })]
    [InlineData("for(i = 0;;) {}", new[] { "(block (; (= i 0))(while True (block )))" })]
    [InlineData("for(; 1 < 10;) {}", new[] { "(while (< 1 10) (block ))" })]
    [InlineData("for(var i = 0; i < 10;) {}", new[] { "(block (var i = 0)(while (< i 10) (block )))" })]
    [InlineData("for(;; i = i + 1) {}", new[] { "(while True (block (block )(; (= i (+ i 1)))))" })]
    [InlineData("for(var i = 0;; i = i + 1) {}",
        new[] { "(block (var i = 0)(while True (block (block )(; (= i (+ i 1))))))" })]
    [InlineData("for(; 1 < 10; i = i + 1) {}", new[] { "(while (< 1 10) (block (block )(; (= i (+ i 1)))))" })]
    [InlineData("for(var i = 0; i < 10; i = i + 1) {}",
        new[] { "(block (var i = 0)(while (< i 10) (block (block )(; (= i (+ i 1))))))" })]
    [InlineData("for(var i = 0; i < 10; i = i + 1) { var x = i; }",
        new[] { "(block (var i = 0)(while (< i 10) (block (block (var x = i))(; (= i (+ i 1))))))" })]
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
    [InlineData("if(true) {}", new[] { "(if True (block ))" })]
    [InlineData("if(x < 10) {}", new[] { "(if (< x 10) (block ))" })]
    [InlineData("if(x < 10) { x = 0; }", new[] { "(if (< x 10) (block (; (= x 0))))" })]
    [InlineData("if(x < 10) {} else {}", new[] { "(if-else (< x 10) (block ) (block ))" })]
    [InlineData("if(x < 10) {} else if (y < 10) {}", new[] { "(if-else (< x 10) (block ) (if (< y 10) (block )))" })]
    [InlineData("if", new string[] { })]
    [InlineData("if(", new string[] { })]
    [InlineData("if()", new string[] { })]
    [InlineData("if(true) {", new string[] { })]
    [InlineData("if(true) }", new string[] { })]
    public void IfStatementAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("return 1;", new[] { "(return 1)" })]
    [InlineData("return x;", new[] { "(return x)" })]
    [InlineData("return", new string[] { })]
    public void ReturnStatementAstTest(string source, string[] expected)
    {
        AssertAst(source, expected);
    }

    [Theory]
    [InlineData("while(true) {}", new[] { "(while True (block ))" })]
    [InlineData("while(x < 10) {}", new[] { "(while (< x 10) (block ))" })]
    [InlineData("while(x < 10) { x = x + 1; }", new[] { "(while (< x 10) (block (; (= x (+ x 1)))))" })]
    [InlineData("while", new string[] { })]
    [InlineData("while(", new string[] { })]
    [InlineData("while()", new string[] { })]
    [InlineData("while(true) {", new string[] { })]
    [InlineData("while(true) }", new string[] { })]
    public void WhileStatementAstTest(string source, string[] expected)
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
