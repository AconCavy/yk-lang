using Xunit;
using YKLang.Exceptions;

namespace YKLang.Tests;

public class EnvironmentTests
{
    [Fact]
    public void InitializeTest()
    {
        var e = Record.Exception(() =>
        {
            var root = new Environment();
            var child = new Environment(root);
        });

        Assert.Null(e);
    }

    [Fact]
    public void DefineAndGetTest()
    {
        var sut = new Environment();
        sut.Define("v1", 0);
        Assert.Equal(0, sut.Get("v1"));

        sut.Define("v2", "Foo");
        Assert.Equal("Foo", sut.Get("v2"));

        sut.Define("v3", true);
        Assert.Equal(true, sut.Get("v3"));

        sut.Define("v4", false);
        Assert.Equal(false, sut.Get("v4"));

        Assert.Throws<InterpretException>(() => sut.Get("v5"));
    }

    [Fact]
    public void AssignTest()
    {
        var sut = new Environment();
        sut.Define("v1", 0);
        sut.Assign("v1", 1);

        Assert.Equal(1, sut.Get("v1"));
        Assert.Throws<InterpretException>(() => sut.Assign("v2", 2));
    }

    [Fact]
    public void GetAncestorTest()
    {
        var root = new Environment();
        var c1 = new Environment(root);
        var c2 = new Environment(root);
        var c3 = new Environment(c1);

        Assert.Equal(root, c1.GetAncestor(1));
        Assert.Equal(root, c2.GetAncestor(1));
        Assert.Equal(c1, c3.GetAncestor(1));
        Assert.Equal(root, c3.GetAncestor(2));
        Assert.Throws<InterpretException>(() => root.GetAncestor(1));
        Assert.Throws<InterpretException>(() => c1.GetAncestor(2));
    }

    [Fact]
    public void TreeTest()
    {
        var root = new Environment();
        var c1 = new Environment(root);
        var c2 = new Environment(root);
        var c3 = new Environment(c1);

        // Define
        root.Define("v1", 1);
        root.Define("v2", 2);
        c1.Define("v1", 11);
        c2.Define("v2", 21);
        c3.Define("v3", 301);

        Assert.Equal(1, root.Get("v1"));
        Assert.Equal(2, root.Get("v2"));
        Assert.Equal(11, c1.Get("v1"));
        Assert.Equal(21, c2.Get("v2"));
        Assert.Equal(301, c3.Get("v3"));

        // Assign
        c1.Assign("v1", 12, 1);
        Assert.Equal(12, root.Get("v1"));
        Assert.Equal(11, c1.Get("v1"));
        Assert.Equal(12, c1.Get("v1", 1));

        c2.Assign("v1", 22);
        Assert.Equal(22, root.Get("v1"));
        Assert.Throws<InterpretException>(() => c2.Get("v1"));
        Assert.Equal(22, c2.Get("v1", 1));

        c3.Assign("v2", 302, 2);
        Assert.Equal(302, root.Get("v2"));
        Assert.Equal(301, c3.Get("v3"));
        Assert.Equal(302, c3.Get("v2", 2));
        Assert.Equal(11, c3.Get("v1", 1));
        Assert.Equal(22, c3.Get("v1", 2));
    }
}
