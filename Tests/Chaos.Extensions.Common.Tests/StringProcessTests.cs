using FluentAssertions;

namespace Chaos.Extensions.Common.Tests;

public class StringProcessTests
{
    [Fact]
    public void Test1()
    {
        const string str = "{{One}} is {One}";

        var result = str.Process(1);

        result.Should().Be("{One} is 1");
    }
}