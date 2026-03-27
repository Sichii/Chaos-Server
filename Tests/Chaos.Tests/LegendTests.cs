#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Legend;
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class LegendTests
{
    #region Construction
    [Test]
    public void Constructor_ShouldAcceptInitialMarks()
    {
        var marks = new[]
        {
            CreateMark("mark1", "Mark 1"),
            CreateMark("mark2", "Mark 2")
        };

        var legend = new Legend(marks);

        legend.ContainsKey("mark1")
              .Should()
              .BeTrue();

        legend.ContainsKey("mark2")
              .Should()
              .BeTrue();
    }
    #endregion

    private static LegendMark CreateMark(string key, string text = "Test Mark", int count = 1)
        => new(
            text,
            key,
            MarkIcon.Warrior,
            MarkColor.Blue,
            count,
            GameTime.Now);

    #region AddOrAccumulate
    [Test]
    public void AddOrAccumulate_ShouldAddNewMark()
    {
        var legend = new Legend();
        var mark = CreateMark("test");

        legend.AddOrAccumulate(mark)
              .Should()
              .BeTrue();

        legend.ContainsKey("test")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddOrAccumulate_ShouldIncrementCount_WhenMarkExists()
    {
        var legend = new Legend();
        var mark = CreateMark("test");
        legend.AddOrAccumulate(mark);

        var mark2 = CreateMark("test", "Updated Text");
        legend.AddOrAccumulate(mark2);

        legend.GetCount("test")
              .Should()
              .Be(2);
    }

    [Test]
    public void AddOrAccumulate_ShouldUpdateText_WhenMarkExists()
    {
        var legend = new Legend();
        legend.AddOrAccumulate(CreateMark("test", "Original"));

        legend.AddOrAccumulate(CreateMark("test", "Updated"));

        legend.TryGetValue("test", out var mark);

        mark!.Text
             .Should()
             .Be("Updated");
    }
    #endregion

    #region AddUnique
    [Test]
    public void AddUnique_ShouldAddMark()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test"));

        legend.ContainsKey("test")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddUnique_ShouldReplace_WhenKeyExists()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test", "Original", 5));

        var replacement = CreateMark("test", "Replaced");
        legend.AddUnique(replacement);

        legend.TryGetValue("test", out var mark);

        mark!.Text
             .Should()
             .Be("Replaced");

        mark.Count
            .Should()
            .Be(1);
    }
    #endregion

    #region Contains / ContainsKey
    [Test]
    public void Contains_ShouldReturnTrue_WhenMarkExists()
    {
        var legend = new Legend();
        var mark = CreateMark("test");
        legend.AddUnique(mark);

        legend.Contains(mark)
              .Should()
              .BeTrue();
    }

    [Test]
    public void ContainsKey_ShouldReturnFalse_WhenKeyNotPresent()
    {
        var legend = new Legend();

        legend.ContainsKey("nonexistent")
              .Should()
              .BeFalse();
    }

    [Test]
    public void ContainsKey_ShouldBeCaseInsensitive()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("TestKey"));

        legend.ContainsKey("testkey")
              .Should()
              .BeTrue();
    }
    #endregion

    #region GetCount
    [Test]
    public void GetCount_ShouldReturnZero_WhenKeyNotPresent()
    {
        var legend = new Legend();

        legend.GetCount("nonexistent")
              .Should()
              .Be(0);
    }

    [Test]
    public void GetCount_ShouldReturnMarkCount()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test", count: 5));

        legend.GetCount("test")
              .Should()
              .Be(5);
    }
    #endregion

    #region Remove / RemoveCount
    [Test]
    public void Remove_ShouldReturnTrue_WhenMarkExists()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test"));

        legend.Remove("test", out var mark)
              .Should()
              .BeTrue();

        mark.Should()
            .NotBeNull();

        legend.ContainsKey("test")
              .Should()
              .BeFalse();
    }

    [Test]
    public void Remove_ShouldReturnFalse_WhenKeyNotPresent()
    {
        var legend = new Legend();

        legend.Remove("nonexistent", out var mark)
              .Should()
              .BeFalse();

        mark.Should()
            .BeNull();
    }

    [Test]
    public void RemoveCount_ShouldDecrementCount()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test", count: 5));

        legend.RemoveCount("test", 2, out var mark)
              .Should()
              .BeTrue();

        mark!.Count
             .Should()
             .Be(3);
    }

    [Test]
    public void RemoveCount_ShouldRemoveMark_WhenCountReachesZero()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test", count: 3));

        legend.RemoveCount("test", 3, out _)
              .Should()
              .BeTrue();

        legend.ContainsKey("test")
              .Should()
              .BeFalse();
    }

    [Test]
    public void RemoveCount_ShouldRemoveMark_WhenCountGoesBelowZero()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test", count: 2));

        legend.RemoveCount("test", 5, out _)
              .Should()
              .BeTrue();

        legend.ContainsKey("test")
              .Should()
              .BeFalse();
    }

    [Test]
    public void RemoveCount_ShouldReturnFalse_WhenKeyNotPresent()
    {
        var legend = new Legend();

        legend.RemoveCount("nonexistent", 1, out var mark)
              .Should()
              .BeFalse();

        mark.Should()
            .BeNull();
    }
    #endregion

    #region TryGetValue
    [Test]
    public void TryGetValue_ShouldReturnTrue_WhenFound()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("test"));

        legend.TryGetValue("test", out var mark)
              .Should()
              .BeTrue();

        mark.Should()
            .NotBeNull();
    }

    [Test]
    public void TryGetValue_ShouldReturnFalse_WhenNotFound()
    {
        var legend = new Legend();

        legend.TryGetValue("nonexistent", out var mark)
              .Should()
              .BeFalse();

        mark.Should()
            .BeNull();
    }
    #endregion

    #region Enumeration
    [Test]
    public void GetEnumerator_ShouldReturnMarksOrderedByAdded()
    {
        var legend = new Legend();
        legend.AddUnique(CreateMark("first", "First"));
        legend.AddUnique(CreateMark("second", "Second"));
        legend.AddUnique(CreateMark("third", "Third"));

        var marks = legend.ToList();

        marks.Should()
             .HaveCount(3);
    }

    [Test]
    public void GetEnumerator_ShouldReturnEmpty_WhenLegendIsEmpty()
    {
        var legend = new Legend();

        legend.ToList()
              .Should()
              .BeEmpty();
    }
    #endregion
}