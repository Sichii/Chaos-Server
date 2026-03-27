#region
using Chaos.Collections;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class IgnoreListTests
{
    #region Constructor
    [Test]
    public void Constructor_WithItems_ShouldPopulateList()
    {
        var list = new IgnoreList(
            [
                "One",
                "Two",
                "Three"
            ]);

        list.Count
            .Should()
            .Be(3);

        list.Contains("One")
            .Should()
            .BeTrue();
    }
    #endregion

    #region Count
    [Test]
    public void Count_ShouldReflectNumberOfItems()
    {
        var list = new IgnoreList();

        list.Count
            .Should()
            .Be(0);

        list.Add("A");
        list.Add("B");

        list.Count
            .Should()
            .Be(2);
    }
    #endregion

    #region ToString
    [Test]
    public void ToString_ShouldJoinWithNewlines()
    {
        var list = new IgnoreList(
            [
                "Alpha",
                "Beta"
            ]);

        var result = list.ToString();

        result.Should()
              .Contain("Alpha");

        result.Should()
              .Contain("Beta");

        result.Should()
              .Contain(Environment.NewLine);
    }
    #endregion

    #region Add / Contains
    [Test]
    public void Add_ShouldAddItem()
    {
        var list = new IgnoreList
        {
            "PlayerOne"
        };

        list.Contains("PlayerOne")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Contains_ShouldBeCaseInsensitive()
    {
        var list = new IgnoreList
        {
            "PlayerOne"
        };

        list.Contains("playerone")
            .Should()
            .BeTrue();

        list.Contains("PLAYERONE")
            .Should()
            .BeTrue();
    }
    #endregion

    #region Remove
    [Test]
    public void Remove_ShouldRemoveItem()
    {
        var list = new IgnoreList
        {
            "PlayerOne"
        };

        list.Remove("PlayerOne")
            .Should()
            .BeTrue();

        list.Contains("PlayerOne")
            .Should()
            .BeFalse();
    }

    [Test]
    public void Remove_ShouldReturnFalse_WhenItemNotPresent()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var list = new IgnoreList();

        list.Remove("Nobody")
            .Should()
            .BeFalse();
    }
    #endregion
}