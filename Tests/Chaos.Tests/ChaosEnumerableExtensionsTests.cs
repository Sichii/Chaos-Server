#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class ChaosEnumerableExtensionsTests
{
    private readonly MapInstance Map = MockMapInstance.Create(width: 50, height: 50);

    #region ThatAreVisibleTo
    [Test]
    public void ThatAreVisibleTo_ShouldReturnEmpty_WhenNoneVisible()
    {
        // Creature.CanSee requires entities to be on the map's entity collection
        // So entities not added to the map won't be visible
        var aisling = MockAisling.Create(Map, "Viewer");
        aisling.SetLocation(Map, new Point(5, 5));

        var invisible = MockMonster.Create(Map, "Invisible");
        invisible.SetLocation(Map, new Point(6, 6));

        var entities = new[]
        {
            invisible
        };

        // Without adding to map entity collection, CanSee returns false
        entities.ThatAreVisibleTo(aisling)
                .Should()
                .BeEmpty();
    }
    #endregion

    #region ToSingleStack
    [Test]
    public void ToSingleStack_ShouldCombineCounts()
    {
        var item1 = MockItem.Create("Apple", 3, true);
        var item2 = MockItem.Create("Apple", 5, true);

        var result = new[]
        {
            item1,
            item2
        }.ToSingleStack();

        result.Count
              .Should()
              .Be(8);
    }
    #endregion

    #region ClosestOrDefault
    [Test]
    public void ClosestOrDefault_ShouldReturnClosestEntity()
    {
        var close = MockMonster.Create(Map, "Close");
        close.SetLocation(Map, new Point(5, 5));

        var far = MockMonster.Create(Map, "Far");
        far.SetLocation(Map, new Point(30, 30));

        var entities = new[]
        {
            far,
            close
        };
        IPoint targetPoint = new Point(4, 4);

        var result = entities.ClosestOrDefault(targetPoint);

        result.Should()
              .BeSameAs(close);
    }

    [Test]
    public void ClosestOrDefault_ShouldReturnNull_WhenEmpty()
    {
        var entities = Array.Empty<Monster>();

        var result = entities.ClosestOrDefault(new Point(0, 0));

        result.Should()
              .BeNull();
    }
    #endregion

    #region ThatAreOnPoint
    [Test]
    public void ThatAreOnPoint_ShouldReturnEntitiesAtPoint()
    {
        var onPoint = MockMonster.Create(Map, "OnPoint");
        onPoint.SetLocation(Map, new Point(10, 10));

        var offPoint = MockMonster.Create(Map, "OffPoint");
        offPoint.SetLocation(Map, new Point(20, 20));

        var entities = new[]
        {
            onPoint,
            offPoint
        };

        var result = entities.ThatAreOnPoint(new Point(10, 10))
                             .ToList();

        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(onPoint);
    }

    [Test]
    public void ThatAreOnPoint_ShouldReturnEmpty_WhenNoneAtPoint()
    {
        var monster = MockMonster.Create(Map, "Monster");
        monster.SetLocation(Map, new Point(5, 5));

        var entities = new[]
        {
            monster
        };

        entities.ThatAreOnPoint(new Point(30, 30))
                .Should()
                .BeEmpty();
    }
    #endregion

    #region TopOrDefault
    [Test]
    public void TopOrDefault_ShouldReturnNewestEntity()
    {
        var older = MockMonster.Create(Map, "Older");
        var newer = MockMonster.Create(Map, "Newer");

        var entities = new[]
        {
            older,
            newer
        };

        // The entity created later has a higher Creation value
        var result = entities.TopOrDefault();

        result.Should()
              .BeSameAs(newer);
    }

    [Test]
    public void TopOrDefault_ShouldReturnNull_WhenEmpty()
    {
        var entities = Array.Empty<Monster>();

        entities.TopOrDefault()
                .Should()
                .BeNull();
    }
    #endregion

    #region ThatAreWithinRange (MapEntity extension)
    [Test]
    public void ThatAreWithinRange_WithPoint_ShouldFilterByRange()
    {
        var close = MockMonster.Create(Map, "Close");
        close.SetLocation(Map, new Point(5, 5));

        var far = MockMonster.Create(Map, "Far");
        far.SetLocation(Map, new Point(40, 40));

        var entities = new[]
        {
            close,
            far
        };
        IPoint origin = new Point(5, 6);

        var result = entities.ThatAreWithinRange(origin, 3)
                             .ToList();

        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(close);
    }

    [Test]
    public void ThatAreWithinRange_WithMultiplePoints_ShouldFilterByAnyPoint()
    {
        var monster = MockMonster.Create(Map, "Monster");
        monster.SetLocation(Map, new Point(10, 10));

        var entities = new[]
        {
            monster
        };
        IPoint point1 = new Point(40, 40);
        IPoint point2 = new Point(10, 11);

        var result = entities.ThatAreWithinRange(3, point1, point2)
                             .ToList();

        result.Should()
              .ContainSingle();
    }
    #endregion

    #region ThatCanObserve
    [Test]
    public void ThatCanObserve_ShouldReturnEmpty_WhenNoneCanObserve()
    {
        var target = MockMonster.Create(Map, "Target");
        target.SetLocation(Map, new Point(5, 5));

        var observer = MockMonster.Create(Map, "Observer");
        observer.SetLocation(Map, new Point(6, 6));

        var entities = new[]
        {
            observer
        };

        // Without adding to map entity collection, CanObserve returns false
        entities.ThatCanObserve(target)
                .Should()
                .BeEmpty();
    }

    [Test]
    public void ThatCanObserve_ShouldExcludeNonCreatures()
    {
        var target = MockMonster.Create(Map, "Target");
        target.SetLocation(Map, new Point(5, 5));

        var door = new Door(
            false,
            1,
            Map,
            new Point(6, 6));

        // Door is not a Creature, so it should be excluded by the "obj is Creature" check
        var entities = new VisibleEntity[]
        {
            door
        };

        entities.ThatCanObserve(target)
                .Should()
                .BeEmpty();
    }
    #endregion

    #region FixStacks
    [Test]
    public void FixStacks_ShouldConsolidateSameNamedItems()
    {
        var item1 = MockItem.Create("Apple", 3, true);
        var item2 = MockItem.Create("Apple", 5, true);

        var items = new[]
        {
            item1,
            item2
        };

        var result = items.FixStacks(MockScriptProvider.ItemCloner.Object)
                          .ToList();

        result.Should()
              .ContainSingle();

        result[0]
            .Count
            .Should()
            .Be(8);
    }

    [Test]
    public void FixStacks_ShouldKeepDifferentItemsSeparate()
    {
        var apple = MockItem.Create("Apple", 3, true);
        var orange = MockItem.Create("Orange", 5, true);

        var items = new[]
        {
            apple,
            orange
        };

        var result = items.FixStacks(MockScriptProvider.ItemCloner.Object)
                          .ToList();

        result.Should()
              .HaveCount(2);
    }
    #endregion

    #region WithFilter
    [Test]
    public void WithFilter_ShouldAlwaysPassNonCreatures()
    {
        var source = MockMonster.Create(Map, "Source");
        source.SetLocation(Map, new Point(5, 5));

        var door = new Door(
            false,
            1,
            Map,
            new Point(5, 6));

        var entities = new MapEntity[]
        {
            door
        };

        // Even with a restrictive filter, non-Creature entities pass through
        var result = entities.WithFilter(source, TargetFilter.AislingsOnly)
                             .ToList();

        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(door);
    }

    [Test]
    public void WithFilter_ShouldFilterCreatures_ByTargetFilter()
    {
        var source = MockMonster.Create(Map, "Source");
        source.SetLocation(Map, new Point(5, 5));

        var monster = MockMonster.Create(Map, "Target");
        monster.SetLocation(Map, new Point(5, 6));

        var entities = new MapEntity[]
        {
            monster
        };

        // AislingsOnly should exclude monsters
        var result = entities.WithFilter(source, TargetFilter.AislingsOnly)
                             .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void WithFilter_ShouldPassCreatures_WhenFilterNone()
    {
        var source = MockMonster.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var entities = new MapEntity[]
        {
            target
        };

        var result = entities.WithFilter(source, TargetFilter.None)
                             .ToList();

        result.Should()
              .ContainSingle();
    }
    #endregion

    #region ThatCollideWith
    [Test]
    public void ThatCollideWith_ShouldReturnCollidingCreatures()
    {
        var creature = MockMonster.Create(Map, "Self");
        creature.SetLocation(Map, new Point(5, 5));

        var other = MockMonster.Create(Map, "Other");
        other.SetLocation(Map, new Point(5, 6));

        var entities = new[]
        {
            other
        };

        // Normal type creatures collide with each other
        var result = entities.ThatCollideWith(creature)
                             .ToList();

        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(other);
    }

    [Test]
    public void ThatCollideWith_ShouldExcludeSelf()
    {
        var creature = MockMonster.Create(Map, "Self");
        creature.SetLocation(Map, new Point(5, 5));

        var entities = new[]
        {
            creature
        };

        // Should not include self
        var result = entities.ThatCollideWith(creature)
                             .ToList();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region ThatThisCollidesWith
    [Test]
    public void ThatThisCollidesWith_ShouldReturnCreaturesThatCollidedWith()
    {
        var creature = MockMonster.Create(Map, "Self");
        creature.SetLocation(Map, new Point(5, 5));

        var other = MockMonster.Create(Map, "Other");
        other.SetLocation(Map, new Point(5, 6));

        var entities = new[]
        {
            other
        };

        // creature.WillCollideWith(other) for Normal vs Normal → true
        var result = entities.ThatThisCollidesWith(creature)
                             .ToList();

        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(other);
    }

    [Test]
    public void ThatThisCollidesWith_ShouldExcludeSelf()
    {
        var creature = MockMonster.Create(Map, "Self");

        var entities = new[]
        {
            creature
        };

        var result = entities.ThatThisCollidesWith(creature)
                             .ToList();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region ThatAreInLanternVision
    [Test]
    public void ThatAreInLanternVision_ShouldIncludeAll_WhenMapIsNotDark()
    {
        var monster = MockMonster.Create(Map, "LanternMonster");
        monster.SetLocation(Map, new Point(5, 5));

        var entities = new VisibleEntity[]
        {
            monster
        };

        entities.ThatAreInLanternVision()
                .Should()
                .ContainSingle();
    }

    [Test]
    public void ThatAreInLanternVision_ShouldExclude_WhenDarkMapAndNoIllumination()
    {
        var darkMap = MockMapInstance.Create(setup: m => m.Flags = MapFlags.Darkness);
        var monster = MockMonster.Create(darkMap, "DarkMonster");
        monster.SetLocation(darkMap, new Point(5, 5));

        var entities = new VisibleEntity[]
        {
            monster
        };

        // Dark map with no aislings illuminating → filtered out
        entities.ThatAreInLanternVision()
                .Should()
                .BeEmpty();
    }
    #endregion

    #region ThatAreWithinRange (ILocation overload)
    [Test]
    public void ThatAreWithinRange_WithLocation_ShouldFilterByRange()
    {
        var close = MockMonster.Create(Map, "Close");
        close.SetLocation(Map, new Point(5, 5));

        var far = MockMonster.Create(Map, "Far");
        far.SetLocation(Map, new Point(40, 40));

        var entities = new[]
        {
            close,
            far
        };

        // Use close as an ILocation source (same map)
        var result = entities.ThatAreWithinRange(close, 3)
                             .ToList();

        // close is within range 0 of itself, far is not within range 3
        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(close);
    }

    [Test]
    public void ThatAreWithinRange_WithMultipleLocations_ShouldFilterByAnyLocation()
    {
        var monster = MockMonster.Create(Map, "Monster");
        monster.SetLocation(Map, new Point(10, 10));

        var nearMonster = MockMonster.Create(Map, "Near");
        nearMonster.SetLocation(Map, new Point(10, 11));

        var farMonster = MockMonster.Create(Map, "Far");
        farMonster.SetLocation(Map, new Point(40, 40));

        var entities = new[]
        {
            monster
        };

        // Check range against multiple ILocation points
        var result = entities.ThatAreWithinRange(3, farMonster, nearMonster)
                             .ToList();

        result.Should()
              .ContainSingle();
    }
    #endregion
}