#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class UserOptionsTests
{
    #region ToggleGroup
    [Test]
    public void ToggleGroup_ShouldToggleAllowGroup()
    {
        var options = new UserOptions();

        options.ToggleGroup();

        options.AllowGroup
               .Should()
               .BeFalse();

        options.ToggleGroup();

        options.AllowGroup
               .Should()
               .BeTrue();
    }
    #endregion

    #region Defaults
    [Test]
    public void AllOptions_ShouldDefaultToTrue()
    {
        var options = new UserOptions();

        options.ShowBodyAnimations
               .Should()
               .BeTrue();

        options.ListenToHitSounds
               .Should()
               .BeTrue();

        options.PriorityAnimations
               .Should()
               .BeTrue();

        options.Option4
               .Should()
               .BeTrue();

        options.Option5
               .Should()
               .BeTrue();

        options.AllowExchange
               .Should()
               .BeTrue();

        options.Option8
               .Should()
               .BeTrue();
    }

    [Test]
    public void AllowGroup_ShouldDefaultToTrue()
    {
        var options = new UserOptions();

        options.AllowGroup
               .Should()
               .BeTrue();
    }

    [Test]
    public void SocialStatus_ShouldDefaultToAwake()
    {
        var options = new UserOptions();

        options.SocialStatus
               .Should()
               .Be(SocialStatus.Awake);
    }
    #endregion

    #region Toggle
    [Test]
    public void Toggle_Option1_ShouldToggleShowBodyAnimations()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option1);

        options.ShowBodyAnimations
               .Should()
               .BeFalse();

        options.Toggle(UserOption.Option1);

        options.ShowBodyAnimations
               .Should()
               .BeTrue();
    }

    [Test]
    public void Toggle_Option2_ShouldToggleListenToHitSounds()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option2);

        options.ListenToHitSounds
               .Should()
               .BeFalse();
    }

    [Test]
    public void Toggle_Option3_ShouldTogglePriorityAnimations()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option3);

        options.PriorityAnimations
               .Should()
               .BeFalse();
    }

    [Test]
    public void Toggle_Option4_ShouldToggleOption4()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option4);

        options.Option4
               .Should()
               .BeFalse();

        options.Toggle(UserOption.Option4);

        options.Option4
               .Should()
               .BeTrue();
    }

    [Test]
    public void Toggle_Option5_ShouldToggleOption5()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option5);

        options.Option5
               .Should()
               .BeFalse();

        options.Toggle(UserOption.Option5);

        options.Option5
               .Should()
               .BeTrue();
    }

    [Test]
    public void Toggle_Option6_ShouldToggleAllowExchange()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option6);

        options.AllowExchange
               .Should()
               .BeFalse();
    }

    [Test]
    public void Toggle_Option7_ShouldDoNothing()
    {
        var options = new UserOptions();

        // Option7 is not used; toggling should not throw
        var act = () => options.Toggle(UserOption.Option7);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Toggle_Option8_ShouldToggleOption8()
    {
        var options = new UserOptions();

        options.Toggle(UserOption.Option8);

        options.Option8
               .Should()
               .BeFalse();

        options.Toggle(UserOption.Option8);

        options.Option8
               .Should()
               .BeTrue();
    }

    [Test]
    public void Toggle_InvalidOption_ShouldThrow()
    {
        var options = new UserOptions();

        var act = () => options.Toggle((UserOption)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region ToString(UserOption)
    [Test]
    public void ToString_WithSpecificOption_ShouldFormatCorrectly()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Option1);

        // Format: "{(byte)opt}{description}         :ON " (padded to 25:3)
        result.Should()
              .Contain("Show body animations");

        result.Should()
              .Contain("ON");
    }

    [Test]
    public void ToString_WithDisabledOption_ShouldShowOff()
    {
        var options = new UserOptions();
        options.Toggle(UserOption.Option1);

        var result = options.ToString(UserOption.Option1);

        result.Should()
              .Contain("OFF");
    }

    [Test]
    public void ToString_WithRequest_ShouldReturnAllOptions()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Request);
        var allResult = options.ToString();

        result.Should()
              .Be(allResult);
    }

    [Test]
    public void ToString_Option4_ShouldContainDescription()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Option4);

        result.Should()
              .Contain("Option 4");

        result.Should()
              .Contain("ON");
    }

    [Test]
    public void ToString_Option5_ShouldContainDescription()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Option5);

        result.Should()
              .Contain("Option 5");

        result.Should()
              .Contain("ON");
    }

    [Test]
    public void ToString_Option7_ShouldHaveEmptyDescription()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Option7);

        // Option7 has empty description and is always false
        result.Should()
              .Contain("OFF");
    }

    [Test]
    public void ToString_Option8_ShouldContainDescription()
    {
        var options = new UserOptions();

        var result = options.ToString(UserOption.Option8);

        result.Should()
              .Contain("Option 8");

        result.Should()
              .Contain("ON");
    }
    #endregion

    #region ToString()
    [Test]
    public void ToString_ShouldStartWithZero()
    {
        var options = new UserOptions();

        var result = options.ToString();

        result.Should()
              .StartWith("0");
    }

    [Test]
    public void ToString_ShouldContainTabSeparatedOptions()
    {
        var options = new UserOptions();

        var result = options.ToString();

        // All 8 options are joined with tabs
        result.Should()
              .Contain("\t");

        // Each option's description should be present (with first char removed)
        result.Should()
              .Contain("Show body animations");
    }
    #endregion
}