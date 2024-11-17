#region
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class SpanExtensionsTests
{
    [Test]
    [Arguments("HelloWorld", "HELLO", true)]
    [Arguments("HelloWorld", "WRLD", false)]
    [Arguments("HelloWorld", "World", true)]
    public void ContainsI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.ContainsI(str);

        result.Should()
              .Be(expectedResult);
    }

    [Test]
    [Arguments("HelloWorld", "WORLD", true)]
    [Arguments("HelloWorld", "world", true)]
    [Arguments("HelloWorld", "Hello", false)]
    public void EndsWithI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.EndsWithI(str);

        result.Should()
              .Be(expectedResult);
    }

    [Test]
    [Arguments("HelloWorld", "HELLO", true)]
    [Arguments("HelloWorld", "hello", true)]
    [Arguments("HelloWorld", "World", false)]
    public void StartsWithI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.StartsWithI(str);

        result.Should()
              .Be(expectedResult);
    }
}