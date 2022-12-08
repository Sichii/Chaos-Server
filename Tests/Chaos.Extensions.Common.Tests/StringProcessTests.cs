using FluentAssertions;

namespace Chaos.Extensions.Common.Tests;

public sealed class StringProcessTests
{
    [Fact]
    public void Test1()
    {
        const string STR = "{{One}} is {One}";

        var result = STR.Inject(1);

        result.Should().Be("{One} is 1");
    }
}