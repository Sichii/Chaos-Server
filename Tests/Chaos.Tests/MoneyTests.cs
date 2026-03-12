#region
using Chaos.Models.World;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class MoneyTests
{
    #region GetSprite
    [Test]
    public void GetSprite_ShouldReturn140_WhenAmountAtLeast5000()
    {
        Money.GetSprite(5000)
             .Should()
             .Be(140);

        Money.GetSprite(10000)
             .Should()
             .Be(140);
    }

    [Test]
    public void GetSprite_ShouldReturn141_WhenAmountAtLeast1000()
    {
        Money.GetSprite(1000)
             .Should()
             .Be(141);

        Money.GetSprite(4999)
             .Should()
             .Be(141);
    }

    [Test]
    public void GetSprite_ShouldReturn142_WhenAmountAtLeast500()
    {
        Money.GetSprite(500)
             .Should()
             .Be(142);

        Money.GetSprite(999)
             .Should()
             .Be(142);
    }

    [Test]
    public void GetSprite_ShouldReturn137_WhenAmountAtLeast100()
    {
        Money.GetSprite(100)
             .Should()
             .Be(137);

        Money.GetSprite(499)
             .Should()
             .Be(137);
    }

    [Test]
    public void GetSprite_ShouldReturn138_WhenAmountAtLeast1()
    {
        Money.GetSprite(1)
             .Should()
             .Be(138);

        Money.GetSprite(99)
             .Should()
             .Be(138);
    }

    [Test]
    public void GetSprite_ShouldReturn139_WhenAmountIsZeroOrNegative()
    {
        Money.GetSprite(0)
             .Should()
             .Be(139);

        Money.GetSprite(-1)
             .Should()
             .Be(139);
    }
    #endregion
}