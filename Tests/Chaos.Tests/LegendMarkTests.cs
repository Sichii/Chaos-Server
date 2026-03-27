#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Legend;
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class LegendMarkTests
{
    private static LegendMark CreateMark(string text = "Test", string key = "testKey", int count = 1)
        => new(
            text,
            key,
            MarkIcon.Yay,
            MarkColor.White,
            count,
            GameTime.Now);

    #region GetHashCode
    [Test]
    public void GetHashCode_ShouldBeBasedOnKey_CaseInsensitive()
    {
        var mark1 = new LegendMark(
            "Text",
            "myKey",
            MarkIcon.Yay,
            MarkColor.White,
            1,
            GameTime.Now);

        var mark2 = new LegendMark(
            "Other",
            "MYKEY",
            MarkIcon.Heart,
            MarkColor.Cyan,
            5,
            GameTime.Now);

        mark1.GetHashCode()
             .Should()
             .Be(mark2.GetHashCode());
    }
    #endregion

    #region Equals
    [Test]
    public void Equals_SameTextDifferentCase_ShouldBeTrue()
    {
        var mark1 = CreateMark("Hero of Mileth");
        var mark2 = CreateMark("hero of mileth");

        mark1.Equals(mark2)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Equals_DifferentText_ShouldBeFalse()
    {
        var mark1 = CreateMark("Hero of Mileth");
        var mark2 = CreateMark("Slayer of Dragons");

        mark1.Equals(mark2)
             .Should()
             .BeFalse();
    }

    [Test]
    public void Equals_Null_ShouldBeFalse()
    {
        var mark = CreateMark();

        mark.Equals(null)
            .Should()
            .BeFalse();
    }
    #endregion

    #region ToString
    [Test]
    public void ToString_WithCount1_ShouldNotShowCount()
    {
        var mark = CreateMark("Hero of Mileth", count: 1);

        var str = mark.ToString();

        str.Should()
           .StartWith("Hero of Mileth -");

        str.Should()
           .NotContain("(1)");
    }

    [Test]
    public void ToString_WithCountGreaterThan1_ShouldShowCount()
    {
        var mark = CreateMark("Monster Kill", count: 5);

        var str = mark.ToString();

        str.Should()
           .Contain("(5)");
    }
    #endregion

    #region Equals (reference equality)
    [Test]
    public void Equals_SameInstance_ShouldReturnTrue()
    {
        var mark = CreateMark("Hero of Mileth");

        mark.Equals(mark)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Equals_ObjectOverload_WithNonLegendMark_ShouldReturnFalse()
    {
        var mark = CreateMark("Hero of Mileth");

        mark.Equals("not a legend mark")
            .Should()
            .BeFalse();
    }
    #endregion
}