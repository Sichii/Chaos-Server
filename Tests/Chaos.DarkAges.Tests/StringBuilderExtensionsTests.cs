#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

namespace Chaos.DarkAges.Tests;

public sealed class StringBuilderExtensionsTests
{
    [Test]
    public void AllMethods_ReturnSameStringBuilder_ForChaining()
    {
        var sb = new StringBuilder();

        var result1 = sb.AppendColorPrefix(MessageColor.Blue);
        var result2 = sb.AppendLineF("test");
        var result3 = sb.AppendColored(MessageColor.Red, "x");
        var result4 = sb.AppendLineFColored(MessageColor.Gray, "y");

        result1.Should()
               .BeSameAs(sb);

        result2.Should()
               .BeSameAs(sb);

        result3.Should()
               .BeSameAs(sb);

        result4.Should()
               .BeSameAs(sb);
    }

    [Test]
    public void AppendColored_WithDefaultColor_AppendsPrefixMessageAndDefaultPrefix()
    {
        var sb = new StringBuilder();

        sb.AppendColored(MessageColor.Red, "test", MessageColor.Gray);

        var expected = MessageColor.Red.ToPrefix() + "test" + MessageColor.Gray.ToPrefix();

        sb.ToString()
          .Should()
          .Be(expected);
    }

    [Test]
    public void AppendColored_WithoutDefaultColor_AppendsPrefixAndMessageOnly()
    {
        var sb = new StringBuilder();

        sb.AppendColored(MessageColor.Red, "test");

        var expected = MessageColor.Red.ToPrefix() + "test";

        sb.ToString()
          .Should()
          .Be(expected);
    }

    [Test]
    public void AppendColorPrefix_AppendsCorrectPrefix()
    {
        var sb = new StringBuilder();

        sb.AppendColorPrefix(MessageColor.Blue);

        sb.ToString()
          .Should()
          .Be("{=" + (char)MessageColor.Blue);
    }

    [Test]
    public void AppendLineF_AppendsValueFollowedByNewline()
    {
        var sb = new StringBuilder();

        sb.AppendLineF("hello");

        sb.ToString()
          .Should()
          .Be("hello\n");
    }

    [Test]
    public void AppendLineF_WithNull_AppendsOnlyNewline()
    {
        var sb = new StringBuilder();

        sb.AppendLineF(null);

        sb.ToString()
          .Should()
          .Be("\n");
    }

    [Test]
    public void AppendLineFColored_WithDefaultColor_AppendsPrefixMessageNewlineAndDefaultPrefix()
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Blue, "msg", MessageColor.Gray);

        var expected = MessageColor.Blue.ToPrefix() + "msg\n" + MessageColor.Gray.ToPrefix();

        sb.ToString()
          .Should()
          .Be(expected);
    }

    [Test]
    public void AppendLineFColored_WithoutDefaultColor_AppendsPrefixMessageAndNewlineOnly()
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Blue, "msg");

        var expected = MessageColor.Blue.ToPrefix() + "msg\n";

        sb.ToString()
          .Should()
          .Be(expected);
    }
}