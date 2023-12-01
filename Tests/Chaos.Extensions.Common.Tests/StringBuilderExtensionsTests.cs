using System.Text;
using Chaos.Common.Definitions;
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class StringBuilderExtensionsTests
{
    [Fact]
    public void AppendColored_NoDefaultColor_ShouldAppendMessageWithoutDefaultColor()
    {
        var sb = new StringBuilder();
        sb.AppendColored(MessageColor.Red, "Hello");

        sb.ToString()
          .Should()
          .Be($"{MessageColor.Red.ToPrefix()}Hello");
    }

    [Fact]
    public void AppendColored_ShouldAppendColoredMessageCorrectly()
    {
        var sb = new StringBuilder();
        sb.AppendColored(MessageColor.Red, "Hello", MessageColor.Blue);

        sb.ToString()
          .Should()
          .Be($"{MessageColor.Red.ToPrefix()}Hello{MessageColor.Blue.ToPrefix()}");
    }

    [Fact]
    public void AppendColorPrefix_ShouldAppendPrefixCorrectly()
    {
        var sb = new StringBuilder();
        sb.AppendColorPrefix(MessageColor.Red);

        sb.ToString()
          .Should()
          .Be($"{MessageColor.Red.ToPrefix()}");
    }

    [Fact]
    public void AppendLineF_ShouldAppendMessageWithLineFeed()
    {
        var sb = new StringBuilder();
        sb.AppendLineF("Hello");

        sb.ToString()
          .Should()
          .Be("Hello\n");
    }

    [Fact]
    public void AppendLineFColored_NoDefaultColor_ShouldAppendMessageWithLineFeedWithoutDefaultColor()
    {
        var sb = new StringBuilder();
        sb.AppendLineFColored(MessageColor.Red, "Hello");

        sb.ToString()
          .Should()
          .Be($"{MessageColor.Red.ToPrefix()}Hello\n");
    }

    [Fact]
    public void AppendLineFColored_ShouldAppendColoredMessageWithLineFeed()
    {
        var sb = new StringBuilder();
        sb.AppendLineFColored(MessageColor.Red, "Hello", MessageColor.Blue);

        sb.ToString()
          .Should()
          .Be($"{MessageColor.Red.ToPrefix()}Hello\n{MessageColor.Blue.ToPrefix()}");
    }
}