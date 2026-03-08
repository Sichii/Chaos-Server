#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

namespace Chaos.DarkAges.Tests;

public sealed class MessageColorExtensionsTests
{
    //@formatter:off
    [Test]
    [Arguments(MessageColor.Gray)]
    [Arguments(MessageColor.Red)]
    [Arguments(MessageColor.Yellow)]
    [Arguments(MessageColor.DarkGreen)]
    [Arguments(MessageColor.Silver)]
    [Arguments(MessageColor.Gainsboro)]
    [Arguments(MessageColor.SpanishGray)]
    [Arguments(MessageColor.Nickel)]
    [Arguments(MessageColor.Slate)]
    [Arguments(MessageColor.Charcoal)]
    [Arguments(MessageColor.DirtyBlack)]
    [Arguments(MessageColor.Black)]
    [Arguments(MessageColor.HotPink)]
    [Arguments(MessageColor.Purple)]
    [Arguments(MessageColor.NeonGreen)]
    [Arguments(MessageColor.Orange)]
    [Arguments(MessageColor.Brown)]
    [Arguments(MessageColor.White)]
    [Arguments(MessageColor.Invisible)]
    //@formatter:on
    public void ToPrefix_AllNonDefaultValues_ReturnsCorrectPrefix(MessageColor color)
        => color.ToPrefix()
                .Should()
                .Be("{=" + (char)color);

    [Test]
    public void ToPrefix_Default_Returns_Empty()
        => MessageColor.Default
                       .ToPrefix()
                       .Should()
                       .BeEmpty();

    [Test]
    public void ToPrefix_NonDefault_Returns_Prefix()
        => MessageColor.Blue
                       .ToPrefix()
                       .Should()
                       .Be("{=" + (char)MessageColor.Blue);
}