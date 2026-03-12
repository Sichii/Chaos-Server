#region
using Chaos.Collections;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class TitleListTests
{
    #region Indexer
    [Test]
    public void Indexer_ShouldReturnItemAtIndex()
    {
        var list = new TitleList(
            [
                "First",
                "Second",
                "Third"
            ]);

        list[0]
            .Should()
            .Be("First");

        list[2]
            .Should()
            .Be("Third");
    }
    #endregion

    #region ToString
    [Test]
    public void ToString_ShouldJoinWithNewlines()
    {
        var list = new TitleList(
            [
                "Hero",
                "Legend"
            ]);

        var result = list.ToString();

        result.Should()
              .Contain("Hero");

        result.Should()
              .Contain("Legend");

        result.Should()
              .Contain(Environment.NewLine);
    }
    #endregion

    #region Add
    [Test]
    public void Add_ShouldAddItem()
    {
        var list = new TitleList
        {
            "Warrior"
        };

        list.Count
            .Should()
            .Be(1);

        list.Contains("Warrior")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Add_ShouldNotAddDuplicate_CaseInsensitive()
    {
        var list = new TitleList
        {
            "Warrior",
            "warrior",
            "WARRIOR"
        };

        list.Count
            .Should()
            .Be(1);
    }
    #endregion

    #region Insert
    [Test]
    public void Insert_ShouldInsertAtIndex()
    {
        var list = new TitleList
        {
            "Alpha",
            "Gamma"
        };

        list.Insert(1, "Beta");

        list[1]
            .Should()
            .Be("Beta");
    }

    [Test]
    public void Insert_ShouldNotInsertDuplicate_CaseInsensitive()
    {
        var list = new TitleList
        {
            "Alpha"
        };

        list.Insert(0, "alpha");

        list.Count
            .Should()
            .Be(1);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_WithDuplicates_ShouldDeduplicate()
    {
        var list = new TitleList(
            [
                "Warrior",
                "warrior",
                "WARRIOR"
            ]);

        list.Count
            .Should()
            .Be(1);
    }

    [Test]
    public void Constructor_WithNull_ShouldCreateEmpty()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var list = new TitleList();

        list.Count
            .Should()
            .Be(0);
    }
    #endregion
}