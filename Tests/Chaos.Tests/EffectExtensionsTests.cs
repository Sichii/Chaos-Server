#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Scripting.EffectScripts.Abstractions;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class EffectExtensionsTests
{
    private static IEffect CreateEffectWithRemaining(double totalSeconds)
    {
        var mock = new Mock<IEffect>();

        mock.Setup(e => e.Remaining)
            .Returns(TimeSpan.FromSeconds(totalSeconds));

        return mock.Object;
    }

    #region GetColor
    [Test]
    public void GetColor_ShouldReturnWhite_WhenRemainingAtLeast60Seconds()
    {
        var effect = CreateEffectWithRemaining(60);

        effect.GetColor()
              .Should()
              .Be(EffectColor.White);
    }

    [Test]
    public void GetColor_ShouldReturnWhite_WhenRemainingOver60Seconds()
    {
        var effect = CreateEffectWithRemaining(120);

        effect.GetColor()
              .Should()
              .Be(EffectColor.White);
    }

    [Test]
    public void GetColor_ShouldReturnRed_WhenRemainingAtLeast30Seconds()
    {
        var effect = CreateEffectWithRemaining(30);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Red);
    }

    [Test]
    public void GetColor_ShouldReturnRed_WhenRemaining45Seconds()
    {
        var effect = CreateEffectWithRemaining(45);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Red);
    }

    [Test]
    public void GetColor_ShouldReturnOrange_WhenRemainingAtLeast15Seconds()
    {
        var effect = CreateEffectWithRemaining(15);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Orange);
    }

    [Test]
    public void GetColor_ShouldReturnYellow_WhenRemainingAtLeast10Seconds()
    {
        var effect = CreateEffectWithRemaining(10);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Yellow);
    }

    [Test]
    public void GetColor_ShouldReturnGreen_WhenRemainingAtLeast5Seconds()
    {
        var effect = CreateEffectWithRemaining(5);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Green);
    }

    [Test]
    public void GetColor_ShouldReturnBlue_WhenRemainingAboveZero()
    {
        var effect = CreateEffectWithRemaining(1);

        effect.GetColor()
              .Should()
              .Be(EffectColor.Blue);
    }

    [Test]
    public void GetColor_ShouldReturnNone_WhenRemainingIsZero()
    {
        var effect = CreateEffectWithRemaining(0);

        effect.GetColor()
              .Should()
              .Be(EffectColor.None);
    }

    [Test]
    public void GetColor_ShouldReturnNone_WhenRemainingIsNegative()
    {
        var effect = CreateEffectWithRemaining(-5);

        effect.GetColor()
              .Should()
              .Be(EffectColor.None);
    }
    #endregion
}