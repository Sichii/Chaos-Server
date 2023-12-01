using FluentAssertions;
using Xunit;

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class SpanExtensionsTests
{
    [Theory]
    [InlineData("HelloWorld", "HELLO", true)]
    [InlineData("HelloWorld", "WRLD", false)]
    [InlineData("HelloWorld", "World", true)]
    public void ContainsI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.ContainsI(str);

        result.Should()
              .Be(expectedResult);
    }

    [Theory]
    [InlineData("HelloWorld", "WORLD", true)]
    [InlineData("HelloWorld", "world", true)]
    [InlineData("HelloWorld", "Hello", false)]
    public void EndsWithI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.EndsWithI(str);

        result.Should()
              .Be(expectedResult);
    }

    [Theory]
    [InlineData("HelloWorld", "HELLO", true)]
    [InlineData("HelloWorld", "hello", true)]
    [InlineData("HelloWorld", "World", false)]
    public void StartsWithI_ShouldWorkCorrectly(string source, string toCheck, bool expectedResult)
    {
        var span = source.AsSpan();
        var str = toCheck.AsSpan();

        var result = span.StartsWithI(str);

        result.Should()
              .Be(expectedResult);
    }
}