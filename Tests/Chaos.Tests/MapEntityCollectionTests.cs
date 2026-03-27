#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MapEntityCollectionTests
{
    private readonly MapEntityCollection Collection;
    private readonly MapInstance Map;

    public MapEntityCollectionTests()
    {
        Map = MockMapInstance.Create(width: 50, height: 50);
        var loggerMock = new Mock<ILogger>();
        Collection = new MapEntityCollection(loggerMock.Object, 50, 50);
    }

    #region AtPoints (multiple points)
    [Test]
    public void AtPoints_ShouldReturnEntitiesFromMultiplePoints()
    {
        var aisling1 = MockAisling.Create(Map, position: new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, position: new Point(10, 10));

        Collection.Add(aisling1.Id, aisling1);
        Collection.Add(aisling2.Id, aisling2);

        var result = Collection.AtPoints<Aisling>(new Point(5, 5), new Point(10, 10))
                               .ToList();

        result.Should()
              .HaveCount(2)
              .And
              .Contain(aisling1)
              .And
              .Contain(aisling2);
    }
    #endregion

    #region Clear
    [Test]
    public void Clear_ShouldRemoveAllEntities()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var monster = MockMonster.Create(Map);
        var door = CreateDoor();

        Collection.Add(aisling.Id, aisling);
        Collection.Add(monster.Id, monster);
        Collection.Add(door.Id, door);

        Collection.Clear();

        Collection.ContainsKey(aisling.Id)
                  .Should()
                  .BeFalse();

        Collection.ContainsKey(monster.Id)
                  .Should()
                  .BeFalse();

        Collection.ContainsKey(door.Id)
                  .Should()
                  .BeFalse();

        Collection.AislingCount
                  .Should()
                  .Be(0);

        Collection.Values<Monster>()
                  .Should()
                  .BeEmpty();
    }
    #endregion

    private Door CreateDoor()
        => new(
            false,
            1,
            Map,
            new Point(3, 3));

    private Money CreateMoney(int amount = 100) => new(amount, Map, new Point(10, 10));

    private ReactorTile CreateReactor()
        => new(
            Map,
            new Point(4, 4),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

    #region Values (OfType fallback)
    [Test]
    public void Values_ShouldReturnMoney_FromGroundEntities()
    {
        var money = CreateMoney();
        Collection.Add(money.Id, money);

        // Money is a GroundEntity but not a GroundItem
        Collection.Values<Money>()
                  .Should()
                  .ContainSingle()
                  .Which
                  .Should()
                  .BeSameAs(money);
    }
    #endregion

    #region Within
    [Test]
    public void Within_ShouldReturnEntitiesInRectangle()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var result = Collection.Within<Aisling>(rect)
                               .ToList();

        result.Should()
              .Contain(aisling);
    }
    #endregion

    #region WithinRange (empty results)
    [Test]
    public void WithinRange_ShouldReturnEmpty_WhenNoEntitiesExist()
    {
        var result = Collection.WithinRange<Monster>(new Point(10, 10), 5)
                               .ToList();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region Add
    [Test]
    public void Add_ShouldAddAisling()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Collection.Add(aisling.Id, aisling);

        Collection.ContainsKey(aisling.Id)
                  .Should()
                  .BeTrue();

        Collection.AislingCount
                  .Should()
                  .Be(1);
    }

    [Test]
    public void Add_ShouldAddMonster()
    {
        var monster = MockMonster.Create(Map);

        Collection.Add(monster.Id, monster);

        Collection.ContainsKey(monster.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<Monster>()
                  .Should()
                  .Contain(monster);
    }

    [Test]
    public void Add_ShouldAddMerchant()
    {
        var merchant = MockMerchant.Create(Map);

        Collection.Add(merchant.Id, merchant);

        Collection.ContainsKey(merchant.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<Merchant>()
                  .Should()
                  .Contain(merchant);
    }

    [Test]
    public void Add_ShouldAddGroundItem()
    {
        var groundItem = MockGroundItem.Create(Map, position: new Point(10, 10));

        Collection.Add(groundItem.Id, groundItem);

        Collection.ContainsKey(groundItem.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<GroundEntity>()
                  .Should()
                  .Contain(groundItem);
    }

    [Test]
    public void Add_ShouldAddDoor()
    {
        var door = CreateDoor();

        Collection.Add(door.Id, door);

        Collection.ContainsKey(door.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<Door>()
                  .Should()
                  .Contain(door);
    }

    [Test]
    public void Add_ShouldAddReactorTile()
    {
        var reactor = CreateReactor();

        Collection.Add(reactor.Id, reactor);

        Collection.ContainsKey(reactor.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<ReactorTile>()
                  .Should()
                  .Contain(reactor);
    }
    #endregion

    #region Remove
    [Test]
    public void Remove_ShouldReturnFalse_WhenEntityDoesNotExist()
        => Collection.Remove(999)
                     .Should()
                     .BeFalse();

    [Test]
    public void Remove_ShouldRemoveAisling()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.Remove(aisling.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(aisling.Id)
                  .Should()
                  .BeFalse();

        Collection.AislingCount
                  .Should()
                  .Be(0);
    }

    [Test]
    public void Remove_ShouldRemoveMonster()
    {
        var monster = MockMonster.Create(Map);
        Collection.Add(monster.Id, monster);

        Collection.Remove(monster.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(monster.Id)
                  .Should()
                  .BeFalse();

        Collection.Values<Monster>()
                  .Should()
                  .BeEmpty();
    }

    [Test]
    public void Remove_ShouldRemoveMerchant()
    {
        var merchant = MockMerchant.Create(Map);
        Collection.Add(merchant.Id, merchant);

        Collection.Remove(merchant.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(merchant.Id)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Remove_ShouldRemoveGroundEntity()
    {
        var groundItem = MockGroundItem.Create(Map, position: new Point(10, 10));
        Collection.Add(groundItem.Id, groundItem);

        Collection.Remove(groundItem.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(groundItem.Id)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Remove_ShouldRemoveDoor()
    {
        var door = CreateDoor();
        Collection.Add(door.Id, door);

        Collection.Remove(door.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(door.Id)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Remove_ShouldRemoveReactorTile()
    {
        var reactor = CreateReactor();
        Collection.Add(reactor.Id, reactor);

        Collection.Remove(reactor.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(reactor.Id)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region ContainsKey
    [Test]
    public void ContainsKey_ShouldReturnTrue_WhenEntityExists()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.ContainsKey(aisling.Id)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void ContainsKey_ShouldReturnFalse_WhenEntityDoesNotExist()
        => Collection.ContainsKey(999)
                     .Should()
                     .BeFalse();
    #endregion

    #region TryGetValue
    [Test]
    public void TryGetValue_ShouldReturnTrue_WhenFoundAndTypeMatches()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.TryGetValue<Aisling>(aisling.Id, out var result)
                  .Should()
                  .BeTrue();

        result.Should()
              .BeSameAs(aisling);
    }

    [Test]
    public void TryGetValue_ShouldReturnFalse_WhenNotFound()
        => Collection.TryGetValue<Aisling>(999, out _)
                     .Should()
                     .BeFalse();

    [Test]
    public void TryGetValue_ShouldReturnFalse_WhenTypeMismatch()
    {
        var monster = MockMonster.Create(Map);
        Collection.Add(monster.Id, monster);

        Collection.TryGetValue<Aisling>(monster.Id, out _)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void TryGetValue_ShouldReturnTrue_WhenBaseTypeRequested()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.TryGetValue<MapEntity>(aisling.Id, out var result)
                  .Should()
                  .BeTrue();

        result.Should()
              .BeSameAs(aisling);
    }
    #endregion

    #region Values
    [Test]
    public void Values_ShouldReturnAislings()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.Values<Aisling>()
                  .Should()
                  .ContainSingle()
                  .Which
                  .Should()
                  .BeSameAs(aisling);
    }

    [Test]
    public void Values_ShouldReturnMonsters()
    {
        var monster = MockMonster.Create(Map);
        Collection.Add(monster.Id, monster);

        Collection.Values<Monster>()
                  .Should()
                  .ContainSingle()
                  .Which
                  .Should()
                  .BeSameAs(monster);
    }

    [Test]
    public void Values_ShouldReturnCreatures_IncludingAllSubtypes()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var monster = MockMonster.Create(Map);
        var merchant = MockMerchant.Create(Map);

        Collection.Add(aisling.Id, aisling);
        Collection.Add(monster.Id, monster);
        Collection.Add(merchant.Id, merchant);

        Collection.Values<Creature>()
                  .Should()
                  .HaveCount(3);
    }

    [Test]
    public void Values_ShouldReturnNamedEntities_IncludingCreaturesAndGroundEntities()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var groundItem = MockGroundItem.Create(Map, position: new Point(10, 10));

        Collection.Add(aisling.Id, aisling);
        Collection.Add(groundItem.Id, groundItem);

        Collection.Values<NamedEntity>()
                  .Should()
                  .HaveCount(2);
    }

    [Test]
    public void Values_ShouldReturnVisibleEntities_IncludingDoors()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var door = CreateDoor();

        Collection.Add(aisling.Id, aisling);
        Collection.Add(door.Id, door);

        Collection.Values<VisibleEntity>()
                  .Should()
                  .HaveCount(2);
    }

    [Test]
    public void Values_ShouldReturnGroundItems_FromGroundEntities()
    {
        var groundItem = MockGroundItem.Create(Map, position: new Point(10, 10));
        var money = CreateMoney();

        Collection.Add(groundItem.Id, groundItem);
        Collection.Add(money.Id, money);

        Collection.Values<GroundItem>()
                  .Should()
                  .ContainSingle()
                  .Which
                  .Should()
                  .BeSameAs(groundItem);
    }

    [Test]
    public void Values_ShouldFallbackToEntityLookup_ForMapEntityType()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var door = CreateDoor();
        var reactor = CreateReactor();

        Collection.Add(aisling.Id, aisling);
        Collection.Add(door.Id, door);
        Collection.Add(reactor.Id, reactor);

        // MapEntity is not specifically cased, so it uses the Default (EntityLookup.Values)
        Collection.Values<MapEntity>()
                  .Should()
                  .HaveCount(3);
    }
    #endregion

    #region AtPoints
    [Test]
    public void AtPoints_ShouldReturnEntitiesAtPoint()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        var result = Collection.AtPoints<Aisling>(new Point(5, 5))
                               .ToList();

        result.Should()
              .Contain(aisling);
    }

    [Test]
    public void AtPoints_ShouldReturnEmpty_WhenPointOutsideBounds()
    {
        var result = Collection.AtPoints<Aisling>(new Point(100, 100))
                               .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void AtPoints_ShouldReturnEmpty_WhenNoEntitiesAtPoint()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        var result = Collection.AtPoints<Aisling>(new Point(20, 20))
                               .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void AtPoints_WithIPoints_ShouldWork()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        IPoint point = new Point(5, 5);

        var result = Collection.AtPoints<Aisling>(point)
                               .ToList();

        result.Should()
              .Contain(aisling);
    }
    #endregion

    #region WithinRange
    [Test]
    public void WithinRange_ShouldReturnEntitiesInRange()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        var result = Collection.WithinRange<Aisling>(new Point(5, 5), 3)
                               .ToList();

        result.Should()
              .Contain(aisling);
    }

    [Test]
    public void WithinRange_ShouldNotReturnEntitiesOutOfRange()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        var result = Collection.WithinRange<Aisling>(new Point(40, 40), 3)
                               .ToList();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region MoveEntity
    [Test]
    public void MoveEntity_ShouldUpdateEntityLocation()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);
        var newPoint = new Point(15, 15);

        Collection.MoveEntity(aisling, newPoint);

        aisling.X
               .Should()
               .Be(15);

        aisling.Y
               .Should()
               .Be(15);
    }

    [Test]
    public void MoveEntity_ShouldBeNoOp_WhenSamePoint()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        // Moving to same point should be a no-op
        Collection.MoveEntity(aisling, new Point(5, 5));

        aisling.X
               .Should()
               .Be(5);

        aisling.Y
               .Should()
               .Be(5);
    }

    [Test]
    public void MoveEntity_ShouldUpdateSpatialIndex()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Collection.Add(aisling.Id, aisling);

        Collection.MoveEntity(aisling, new Point(20, 20));

        // Should find at new location
        Collection.AtPoints<Aisling>(new Point(20, 20))
                  .Should()
                  .Contain(aisling);

        // Should not find at old location
        Collection.AtPoints<Aisling>(new Point(5, 5))
                  .Should()
                  .NotContain(aisling);
    }
    #endregion

    #region Within (empty results)
    [Test]
    public void Within_ShouldReturnEmpty_WhenNoEntitiesInRectangle()
    {
        // Empty collection, query should return nothing
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var result = Collection.Within<Aisling>(rect)
                               .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void Within_ShouldReturnEmpty_WhenEntitiesAreOutsideRectangle()
    {
        var aisling = MockAisling.Create(Map, position: new Point(40, 40));
        Collection.Add(aisling.Id, aisling);

        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var result = Collection.Within<Aisling>(rect)
                               .ToList();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region Add / Remove Money
    [Test]
    public void Add_ShouldAddMoney_AsGroundEntity()
    {
        var money = CreateMoney();

        Collection.Add(money.Id, money);

        Collection.ContainsKey(money.Id)
                  .Should()
                  .BeTrue();

        Collection.Values<GroundEntity>()
                  .Should()
                  .Contain(money);
    }

    [Test]
    public void Remove_ShouldRemoveMoney()
    {
        var money = CreateMoney();
        Collection.Add(money.Id, money);

        Collection.Remove(money.Id)
                  .Should()
                  .BeTrue();

        Collection.ContainsKey(money.Id)
                  .Should()
                  .BeFalse();
    }
    #endregion
}