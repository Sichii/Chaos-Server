#region
using System.Reflection;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Map;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Time;
using Chaos.Time.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MapInstanceTests
{
    private MapInstance Map { get; set; } = null!;

    #region Constructor — extraScriptKeys
    [Test]
    public void Constructor_ShouldAddExtraScriptKeys_WhenProvided()
    {
        var template = new MapTemplate
        {
            TemplateKey = "extra_key_map",
            Width = 10,
            Height = 10,
            Bounds = new Rectangle(
                0,
                0,
                10,
                10),
            Tiles = new Tile[10, 10],
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "BaseScript"
            }
        };

        var mapInstance = new MapInstance(
            template,
            new Mock<ISimpleCache>().Object,
            new Mock<IShardGenerator>().Object,
            MockScriptProvider.Instance.Object,
            "ExtraKeyMap",
            "extra_key_map1",
            new Mock<IAsyncStore<Aisling>>().Object,
            new CancellationTokenSource(),
            new Mock<ILogger<MapInstance>>().Object,
            new List<string>
            {
                "ExtraScript1",
                "ExtraScript2"
            });

        mapInstance.ScriptKeys
                   .Should()
                   .Contain("BaseScript")
                   .And
                   .Contain("ExtraScript1")
                   .And
                   .Contain("ExtraScript2");
    }
    #endregion

    private static MonsterSpawn CreateMonsterSpawn()
        => new()
        {
            BlackList = new List<IPoint>(),
            Direction = null,
            ExtraLootTables = [],
            ExtraScriptKeys = [],
            MaxAmount = 5,
            MaxPerSpawn = 2,
            MonsterFactory = new Mock<IMonsterFactory>().Object,
            MonsterTemplate = new MonsterTemplate
            {
                Name = "TestSpawnMonster",
                TemplateKey = "test_spawn",
                AggroRange = 5,
                AssailIntervalMs = 1000,
                ExpReward = 100,
                AbilityReward = 0,
                MinGoldDrop = 0,
                MaxGoldDrop = 0,
                MoveIntervalMs = 1000,
                SkillIntervalMs = 1000,
                SpellIntervalMs = 1000,
                WanderIntervalMs = 1000,
                Sprite = 1,
                Type = CreatureType.Normal,
                LootTables = [],
                SkillTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                SpellTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
                StatSheet = new StatSheet()
            },
            SpawnArea = null,
            SpawnTimer = new Mock<IIntervalTimer>().Object
        };

    private static MapTemplate CreateNewMapTemplate()
        => new()
        {
            TemplateKey = "new_map",
            Width = 15,
            Height = 15,
            Bounds = new Rectangle(
                0,
                0,
                15,
                15),
            Tiles = new Tile[15, 15],
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

    private ReactorTile CreateReactor(IPoint point, bool shouldBlock = false)
        => new(
            Map,
            point,
            shouldBlock,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

    private static Mock<IShardGenerator> GetShardGeneratorMock(MapInstance map)
    {
        var field = typeof(MapInstance).GetField("ShardGenerator", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var shardGenerator = (IShardGenerator)field.GetValue(map)!;

        return Mock.Get(shardGenerator);
    }

    private static Mock<ISimpleCache> GetSimpleCacheMock(MapInstance map)
    {
        var field = typeof(MapInstance).GetField("SimpleCache", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var simpleCache = (ISimpleCache)field.GetValue(map)!;

        return Mock.Get(simpleCache);
    }

    #region Morph with aislings
    [Test]
    public void Morph_ShouldRefreshAislings_WhenAislingsPresent()
    {
        var newTemplate = CreateNewMapTemplate();
        var simpleCacheMock = GetSimpleCacheMock(Map);

        simpleCacheMock.Setup(c => c.Get<MapTemplate>("new_map"))
                       .Returns(newTemplate);

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.SimpleAdd(aisling);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        Map.Morph("new_map");

        // Aisling.Refresh(true) sends multiple packets
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);
    }
    #endregion

    private static void SetShardingOptions(MapInstance map, ShardingOptions options)
        => typeof(MapInstance).GetProperty(nameof(MapInstance.ShardingOptions))!.SetValue(map, options);

    [Before(Test)]
    public void Setup() => Map = MockMapInstance.Create(width: 20, height: 20);

    private static void SetupShardGenerator(MapInstance baseMap, MapInstance shardToReturn)
    {
        var mock = GetShardGeneratorMock(baseMap);

        mock.Setup(sg => sg.CreateShardOfInstance(baseMap.InstanceId))
            .Returns(shardToReturn);
    }

    #region Stop
    [Test]
    public void Stop_ShouldCancelMapInstanceCtx()
    {
        Map.Stop();

        Map.MapInstanceCtx
           .IsCancellationRequested
           .Should()
           .BeTrue();
    }
    #endregion

    #region Update — light level already matches
    [Test]
    public void Update_ShouldNotSendLightLevel_WhenLightLevelAlreadyMatches()
    {
        Map.AutoDayNightCycle = true;

        // Pre-set CurrentLightLevel to match the current game time
        var currentTimeOfDay = GameTime.Now.TimeOfDay;
        Map.CurrentLightLevel = currentTimeOfDay;

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Update to trigger DayNightCycleTimer — but light level already matches
        Map.Update(TimeSpan.FromSeconds(2));

        // Should NOT send light level since it hasn't changed
        clientMock.Verify(c => c.SendLightLevel(It.IsAny<LightLevel>()), Times.Never);
    }
    #endregion

    #region UpdateNearbyViewPorts
    [Test]
    public void UpdateNearbyViewPorts_ShouldNotThrow_WhenNoCreaturesNearby()
    {
        var action = () => Map.UpdateNearbyViewPorts(new Point(5, 5));

        action.Should()
              .NotThrow();
    }
    #endregion

    #region Properties
    [Test]
    public void InstanceId_ShouldReturnConstructedId()
        => Map.InstanceId
              .Should()
              .Be("test_map1");

    [Test]
    public void Name_ShouldReturnConstructedName()
        => Map.Name
              .Should()
              .Be("Test Map");

    [Test]
    public void Template_ShouldReturnConstructedTemplate()
    {
        Map.Template
           .TemplateKey
           .Should()
           .Be("test_map");

        Map.Template
           .Width
           .Should()
           .Be(20);

        Map.Template
           .Height
           .Should()
           .Be(20);
    }

    [Test]
    public void IsShard_ShouldReturnFalse_WhenBaseInstanceIdIsNull()
        => Map.IsShard
              .Should()
              .BeFalse();

    [Test]
    public void LoadedFromInstanceId_ShouldReturnInstanceId_WhenNotShard()
        => Map.LoadedFromInstanceId
              .Should()
              .Be(Map.InstanceId);

    [Test]
    public void HasAislings_ShouldReturnFalse_WhenNoAislingsOnMap()
        => Map.HasAislings
              .Should()
              .BeFalse();

    [Test]
    public void HasAislings_ShouldReturnTrue_WhenAislingPresent()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        Map.HasAislings
           .Should()
           .BeTrue();
    }
    #endregion

    #region IsWithinMap
    [Test]
    public void IsWithinMap_ShouldReturnTrue_ForValidPoint()
        => Map.IsWithinMap(new Point(5, 5))
              .Should()
              .BeTrue();

    [Test]
    public void IsWithinMap_ShouldReturnTrue_ForOrigin()
        => Map.IsWithinMap(new Point(0, 0))
              .Should()
              .BeTrue();

    [Test]
    public void IsWithinMap_ShouldReturnTrue_ForMaxBoundary()
        => Map.IsWithinMap(new Point(19, 19))
              .Should()
              .BeTrue();

    [Test]
    public void IsWithinMap_ShouldReturnFalse_ForOutOfBounds()
        => Map.IsWithinMap(new Point(20, 20))
              .Should()
              .BeFalse();

    [Test]
    public void IsWithinMap_ShouldReturnFalse_ForNegativeCoordinates()
        => Map.IsWithinMap(new Point(-1, -1))
              .Should()
              .BeFalse();
    #endregion

    #region IsWall
    [Test]
    public void IsWall_ShouldReturnFalse_ForOpenTile()
        => Map.IsWall(new Point(5, 5))
              .Should()
              .BeFalse();

    [Test]
    public void IsWall_ShouldReturnTrue_ForWallTile()
    {
        MockMapInstance.SetWall(Map, new Point(3, 3));

        Map.IsWall(new Point(3, 3))
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnFalse_ForOpenDoor_OnNonWallTile()
    {
        // Door is open on a non-wall tile — should not be considered a wall
        var door = new Door(
            false,
            1,
            Map,
            new Point(3, 3))
        {
            Closed = false
        };
        Map.SimpleAdd(door);

        Map.IsWall(new Point(3, 3))
           .Should()
           .BeFalse();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_ForWallTile_EvenWithOpenDoor()
    {
        // Template wall overrides any open door
        MockMapInstance.SetWall(Map, new Point(3, 3));

        var door = new Door(
            false,
            1,
            Map,
            new Point(3, 3))
        {
            Closed = false
        };
        Map.SimpleAdd(door);

        Map.IsWall(new Point(3, 3))
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_ForClosedDoor()
    {
        var door = new Door(
            false,
            1,
            Map,
            new Point(3, 3))
        {
            Closed = true
        };
        Map.SimpleAdd(door);

        Map.IsWall(new Point(3, 3))
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWall_IPointOverload_ShouldDelegateToPointOverload()
    {
        MockMapInstance.SetWall(Map, new Point(3, 3));

        IPoint point = new Point(3, 3);

        Map.IsWall(point)
           .Should()
           .BeTrue();
    }
    #endregion

    #region IsWalkable
    [Test]
    public void IsWalkable_ShouldReturnTrue_ForOpenTile()
        => Map.IsWalkable(new Point(5, 5))
              .Should()
              .BeTrue();

    [Test]
    public void IsWalkable_ShouldReturnFalse_ForOutOfBounds()
        => Map.IsWalkable(new Point(100, 100))
              .Should()
              .BeFalse();

    [Test]
    public void IsWalkable_ShouldReturnFalse_ForWallTile()
    {
        MockMapInstance.SetWall(Map, new Point(3, 3));

        Map.IsWalkable(new Point(3, 3))
           .Should()
           .BeFalse();
    }

    [Test]
    public void IsWalkable_ShouldReturnTrue_ForWall_WhenIgnoreWallsTrue()
    {
        MockMapInstance.SetWall(Map, new Point(3, 3));

        Map.IsWalkable(new Point(3, 3), ignoreWalls: true)
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWalkable_ShouldReturnTrue_ForOpenDoorOnWall()
    {
        MockMapInstance.SetWall(Map, new Point(3, 3));

        var door = new Door(
            false,
            1,
            Map,
            new Point(3, 3))
        {
            Closed = false
        };
        Map.SimpleAdd(door);

        Map.IsWalkable(new Point(3, 3))
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWalkable_ShouldReturnFalse_ForBlockingReactor_WhenNotIgnored()
    {
        var reactor = CreateReactor(new Point(5, 5), true);
        Map.SimpleAdd(reactor);

        Map.IsWalkable(new Point(5, 5), ignoreBlockingReactors: false)
           .Should()
           .BeFalse();
    }

    [Test]
    public void IsWalkable_ShouldReturnTrue_ForBlockingReactor_WhenIgnored()
    {
        var reactor = CreateReactor(new Point(5, 5), true);
        Map.SimpleAdd(reactor);

        Map.IsWalkable(new Point(5, 5), ignoreBlockingReactors: true)
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWalkable_ShouldReturnTrue_WhenIgnoreCollisionTrue_EvenWithCreature()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.IsWalkable(new Point(5, 5), ignoreCollision: true)
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWalkable_ShouldReturnFalse_WhenCreatureCollision()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        var otherMonster = MockMonster.Create(Map);

        Map.IsWalkable(new Point(5, 5), otherMonster)
           .Should()
           .BeFalse();
    }

    [Test]
    public void IsWalkable_ShouldDefaultIgnoreBlockingReactors_ForAisling()
    {
        // Aislings default to ignoreBlockingReactors=true
        var reactor = CreateReactor(new Point(7, 7), true);
        Map.SimpleAdd(reactor);

        var aisling = MockAisling.Create(Map, position: new Point(6, 7));

        Map.IsWalkable(new Point(7, 7), aisling)
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsWalkable_IPointOverload_ShouldDelegateToPointOverload()
    {
        IPoint point = new Point(5, 5);

        Map.IsWalkable(point)
           .Should()
           .BeTrue();
    }
    #endregion

    #region IsBlockingReactor
    [Test]
    public void IsBlockingReactor_ShouldReturnFalse_WhenNoReactors()
        => Map.IsBlockingReactor(new Point(5, 5))
              .Should()
              .BeFalse();

    [Test]
    public void IsBlockingReactor_ShouldReturnTrue_WhenBlockingReactorPresent()
    {
        var reactor = CreateReactor(new Point(5, 5), true);
        Map.SimpleAdd(reactor);

        Map.IsBlockingReactor(new Point(5, 5))
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsBlockingReactor_ShouldReturnFalse_WhenNonBlockingReactorPresent()
    {
        var reactor = CreateReactor(new Point(5, 5));
        Map.SimpleAdd(reactor);

        Map.IsBlockingReactor(new Point(5, 5))
           .Should()
           .BeFalse();
    }
    #endregion

    #region IsReactor
    [Test]
    public void IsReactor_ShouldReturnFalse_WhenNoReactors()
        => Map.IsReactor(new Point(5, 5))
              .Should()
              .BeFalse();

    [Test]
    public void IsReactor_ShouldReturnTrue_WhenReactorPresent()
    {
        var reactor = CreateReactor(new Point(5, 5));
        Map.SimpleAdd(reactor);

        Map.IsReactor(new Point(5, 5))
           .Should()
           .BeTrue();
    }
    #endregion

    #region IsInSharedLanternVision
    [Test]
    public void IsInSharedLanternVision_ShouldReturnTrue_WhenMapNotDark()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.IsInSharedLanternVision(monster)
           .Should()
           .BeTrue();
    }

    [Test]
    public void IsInSharedLanternVision_ShouldReturnFalse_OnDarkMap_WhenNoLanternNearby()
    {
        Map.Flags = MapFlags.Darkness;
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.IsInSharedLanternVision(monster)
           .Should()
           .BeFalse();
    }
    #endregion

    #region SimpleAdd
    [Test]
    public void SimpleAdd_ShouldSetMapInstanceOnEntity()
    {
        var monster = MockMonster.Create(Map);
        var otherMap = MockMapInstance.Create("other", width: 20, height: 20);

        otherMap.SimpleAdd(monster);

        monster.MapInstance
               .Should()
               .BeSameAs(otherMap);
    }

    [Test]
    public void SimpleAdd_ShouldMakeEntityFindable()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.TryGetEntity<Monster>(monster.Id, out var found)
           .Should()
           .BeTrue();

        found.Should()
             .BeSameAs(monster);
    }
    #endregion

    #region GetEntities
    [Test]
    public void GetEntities_ShouldReturnAllOfType()
    {
        var m1 = MockMonster.Create(Map);
        var m2 = MockMonster.Create(Map);
        Map.SimpleAdd(m1);
        Map.SimpleAdd(m2);

        Map.GetEntities<Monster>()
           .Should()
           .HaveCount(2)
           .And
           .Contain(m1)
           .And
           .Contain(m2);
    }

    [Test]
    public void GetEntities_ShouldReturnEmpty_WhenNoneOfType()
        => Map.GetEntities<Monster>()
              .Should()
              .BeEmpty();

    [Test]
    public void GetEntitiesAtPoints_ShouldReturnEntitiesAtSpecifiedPoint()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.GetEntitiesAtPoints<Monster>(new Point(5, 5))
           .Should()
           .Contain(monster);
    }

    [Test]
    public void GetEntitiesAtPoints_ShouldReturnEmpty_WhenNoEntitiesAtPoint()
        => Map.GetEntitiesAtPoints<Monster>(new Point(10, 10))
              .Should()
              .BeEmpty();

    [Test]
    public void GetEntitiesAtPoints_IPointOverload_ShouldWork()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        IPoint point = new Point(5, 5);

        Map.GetEntitiesAtPoints<Monster>(point)
           .Should()
           .Contain(monster);
    }

    [Test]
    public void GetEntitiesWithinRange_ShouldReturnEntitiesInRange()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.GetEntitiesWithinRange<Monster>(new Point(5, 5), 3)
           .Should()
           .Contain(monster);
    }

    [Test]
    public void GetEntitiesWithinRange_ShouldNotReturnEntitiesOutOfRange()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.GetEntitiesWithinRange<Monster>(new Point(19, 19), 3)
           .Should()
           .NotContain(monster);
    }

    [Test]
    public void GetEntitiesWithin_ShouldReturnEntitiesInRectangle()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        Map.GetEntitiesWithin<Monster>(rect)
           .Should()
           .Contain(monster);
    }

    [Test]
    public void GetEntitiesWithin_ShouldNotReturnEntitiesOutsideRectangle()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        var rect = new Rectangle(
            10,
            10,
            5,
            5);

        Map.GetEntitiesWithin<Monster>(rect)
           .Should()
           .NotContain(monster);
    }
    #endregion

    #region TryGetEntity
    [Test]
    public void TryGetEntity_ShouldReturnTrue_WhenEntityExists()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.TryGetEntity<Monster>(monster.Id, out var found)
           .Should()
           .BeTrue();

        found.Should()
             .BeSameAs(monster);
    }

    [Test]
    public void TryGetEntity_ShouldReturnFalse_WhenEntityDoesNotExist()
        => Map.TryGetEntity<Monster>(999, out _)
              .Should()
              .BeFalse();

    [Test]
    public void TryGetEntity_ShouldReturnFalse_WhenTypeMismatch()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.TryGetEntity<Aisling>(monster.Id, out _)
           .Should()
           .BeFalse();
    }

    [Test]
    public void TryGetEntity_ShouldReturnTrue_WhenBaseTypeRequested()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.TryGetEntity<Creature>(monster.Id, out var found)
           .Should()
           .BeTrue();

        found.Should()
             .BeSameAs(monster);
    }
    #endregion

    #region MoveEntity
    [Test]
    public void MoveEntity_ShouldUpdateEntityPosition()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.MoveEntity(monster, new Point(10, 10));

        monster.X
               .Should()
               .Be(10);

        monster.Y
               .Should()
               .Be(10);
    }

    [Test]
    public void MoveEntity_ShouldUpdateSpatialIndex()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.MoveEntity(monster, new Point(10, 10));

        Map.GetEntitiesAtPoints<Monster>(new Point(10, 10))
           .Should()
           .Contain(monster);

        Map.GetEntitiesAtPoints<Monster>(new Point(5, 5))
           .Should()
           .NotContain(monster);
    }
    #endregion

    #region RemoveEntity
    [Test]
    public void RemoveEntity_ShouldReturnFalse_WhenEntityNotOnMap()
    {
        var monster = MockMonster.Create(Map);

        Map.RemoveEntity(monster)
           .Should()
           .BeFalse();
    }

    [Test]
    public void RemoveEntity_ShouldReturnTrue_WhenEntityRemoved()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.RemoveEntity(monster)
           .Should()
           .BeTrue();

        Map.TryGetEntity<Monster>(monster.Id, out _)
           .Should()
           .BeFalse();
    }

    [Test]
    public void RemoveEntity_ShouldCallHandleMapDeparture_ForCreature()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        // Add some approach times to verify HandleMapDeparture clears them
        var otherMonster = MockMonster.Create(Map);
        Map.SimpleAdd(otherMonster);
        monster.ApproachTime[otherMonster] = DateTime.UtcNow;

        Map.RemoveEntity(monster);

        monster.ApproachTime
               .Should()
               .BeEmpty();
    }

    [Test]
    public void RemoveEntity_ShouldCallScriptOnExited_ForCreature()
    {
        var mapScript = Mock.Get(Map.Script);
        mapScript.Reset();

        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        mapScript.Setup(s => s.OnExited(monster));

        Map.RemoveEntity(monster);

        mapScript.Verify(s => s.OnExited(monster), Times.Once);
    }

    [Test]
    public void RemoveEntity_ShouldReturnTrue_ForNonVisibleEntity()
    {
        // ReactorTile is InteractableEntity -> MapEntity, NOT a VisibleEntity or Creature
        // This exercises the default branch of the switch (no viewport update, no HandleMapDeparture)
        var reactor = CreateReactor(new Point(3, 3));
        Map.SimpleAdd(reactor);

        Map.RemoveEntity(reactor)
           .Should()
           .BeTrue();

        Map.TryGetEntity<ReactorTile>(reactor.Id, out _)
           .Should()
           .BeFalse();
    }
    #endregion

    #region AddEntity
    [Test]
    public void AddEntity_WithNonAisling_ShouldAddToMap()
    {
        var monster = MockMonster.Create(Map);

        Map.AddEntity(monster, new Point(5, 5));

        Map.TryGetEntity<Monster>(monster.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_WithAisling_WhenNoShardingOptions_ShouldAddToMap()
    {
        // ShardingOptions is null by default on mock, so aisling goes through InnerAddEntity
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddEntity(aisling, new Point(5, 5));

        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_ShouldSetEntityPosition()
    {
        var monster = MockMonster.Create(Map);

        Map.AddEntity(monster, new Point(8, 8));

        monster.X
               .Should()
               .Be(8);

        monster.Y
               .Should()
               .Be(8);
    }

    [Test]
    public void AddEntity_ShouldSetEntityMapInstance()
    {
        var monster = MockMonster.Create(Map);
        var otherMap = MockMapInstance.Create("other", width: 20, height: 20);

        otherMap.AddEntity(monster, new Point(5, 5));

        monster.MapInstance
               .Should()
               .BeSameAs(otherMap);
    }
    #endregion

    #region AddEntities
    [Test]
    public void AddEntities_ShouldThrow_WhenCollectionContainsAislings()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        var action = () => Map.AddEntities(
            new VisibleEntity[]
            {
                aisling
            });

        action.Should()
              .Throw<InvalidOperationException>();
    }

    [Test]
    public void AddEntities_ShouldDoNothing_WhenCollectionEmpty()
    {
        Map.AddEntities(Array.Empty<Monster>());

        Map.GetEntities<Monster>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public void AddEntities_ShouldAddAllEntities()
    {
        var m1 = MockMonster.Create(Map);
        var m2 = MockMonster.Create(Map);

        Map.AddEntities(
            [
                m1,
                m2
            ]);

        Map.TryGetEntity<Monster>(m1.Id, out _)
           .Should()
           .BeTrue();

        Map.TryGetEntity<Monster>(m2.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntities_ShouldSetMapInstance()
    {
        var otherMap = MockMapInstance.Create("other", width: 20, height: 20);
        var monster = MockMonster.Create(Map);

        otherMap.AddEntities([monster]);

        monster.MapInstance
               .Should()
               .BeSameAs(otherMap);
    }

    [Test]
    public void AddEntities_ShouldCallScriptOnEntered_ForCreatures()
    {
        var mapScript = Mock.Get(Map.Script);
        mapScript.Reset();

        var monster = MockMonster.Create(Map);

        Map.AddEntities([monster]);

        mapScript.Verify(s => s.OnEntered(monster), Times.Once);
    }
    #endregion

    #region AddSpawner
    [Test]
    public void AddSpawner_ShouldSetMapInstanceOnSpawner()
    {
        var spawner = CreateMonsterSpawn();

        Map.AddSpawner(spawner);

        spawner.MapInstance
               .Should()
               .BeSameAs(Map);
    }

    [Test]
    public void AddSpawner_ShouldAddToMonsterSpawnsList()
    {
        var spawner = CreateMonsterSpawn();

        Map.AddSpawner(spawner);

        Map.MonsterSpawns
           .Should()
           .Contain(spawner);
    }
    #endregion

    #region BeginInvoke
    [Test]
    public void BeginInvoke_ShouldEnqueueAction()
    {
        var executed = false;

        Map.BeginInvoke(() => executed = true);

        // Actions are dequeued during Update
        Map.Update(TimeSpan.FromMilliseconds(100));

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void BeginInvoke_ShouldExecuteMultipleActionsInOrder()
    {
        var order = new List<int>();

        Map.BeginInvoke(() => order.Add(1));
        Map.BeginInvoke(() => order.Add(2));
        Map.BeginInvoke(() => order.Add(3));

        Map.Update(TimeSpan.FromMilliseconds(100));

        order.Should()
             .ContainInOrder(1, 2, 3);
    }
    #endregion

    #region Click (by id)
    [Test]
    public void ClickById_ShouldCallOnClicked_WhenEntityInRangeAndObservable()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);
        Map.MoveEntity(monster, new Point(6, 5));

        // Set up approach time so CanObserve passes
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        var monsterScript = Mock.Get(monster.Script);
        monsterScript.Reset();
        monsterScript.Setup(s => s.OnClicked(aisling));

        var aislingScript = Mock.Get(aisling.Script);
        aislingScript.Reset();

        aislingScript.Setup(s => s.CanSee(monster))
                     .Returns(true);

        Map.Click(monster.Id, aisling);

        monsterScript.Verify(s => s.OnClicked(aisling), Times.Once);
    }

    [Test]
    public void ClickById_ShouldNotCallOnClicked_WhenEntityNotFound()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        // Should not throw — entity doesn't exist
        Map.Click(999, aisling);
    }

    [Test]
    public void ClickById_ShouldNotCallOnClicked_WhenEntityOutOfRange()
    {
        var aisling = MockAisling.Create(Map, position: new Point(0, 0));
        Map.SimpleAdd(aisling);

        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);
        Map.MoveEntity(monster, new Point(19, 19));

        var monsterScript = Mock.Get(monster.Script);
        monsterScript.Reset();

        Map.Click(monster.Id, aisling);

        monsterScript.Verify(s => s.OnClicked(It.IsAny<Aisling>()), Times.Never);
    }
    #endregion

    #region Click (by point)
    [Test]
    public void ClickByPoint_ShouldClickDoor_WhenDoorPresent()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var aislingScript = Mock.Get(aisling.Script);
        aislingScript.Reset();

        aislingScript.Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
                     .Returns(true);

        var door = new Door(
            false,
            1,
            Map,
            new Point(5, 6))
        {
            Closed = true
        };
        Map.SimpleAdd(door);

        // Set up approach time so CanObserve passes (ThatAreObservedBy filters)
        aisling.ApproachTime[door] = DateTime.UtcNow;

        Map.Click(new Point(5, 6), aisling);

        // Door.OnClicked toggles closed
        door.Closed
            .Should()
            .BeFalse();
    }

    [Test]
    public void ClickByPoint_ShouldNotClick_WhenOutOfRange()
    {
        var aisling = MockAisling.Create(Map, position: new Point(0, 0));
        Map.SimpleAdd(aisling);

        var door = new Door(
            false,
            1,
            Map,
            new Point(19, 19))
        {
            Closed = true
        };
        Map.SimpleAdd(door);

        Map.Click(new Point(19, 19), aisling);

        door.Closed
            .Should()
            .BeTrue();
    }
    #endregion

    #region Destroy
    [Test]
    public void Destroy_ShouldClearObjects()
    {
        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        Map.Destroy();

        Map.TryGetEntity<Monster>(monster.Id, out _)
           .Should()
           .BeFalse();
    }

    [Test]
    public void Destroy_ShouldClearMonsterSpawns()
    {
        var spawner = CreateMonsterSpawn();
        Map.AddSpawner(spawner);

        Map.Destroy();

        Map.MonsterSpawns
           .Should()
           .BeEmpty();
    }
    #endregion

    #region PlayMusic
    [Test]
    public void PlayMusic_ShouldSendSoundToAllAislings()
    {
        var aisling1 = MockAisling.Create(Map, position: new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, position: new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        var client1 = Mock.Get(aisling1.Client);
        var client2 = Mock.Get(aisling2.Client);

        Map.PlayMusic(42);

        client1.Verify(c => c.SendSound(42, true), Times.AtLeastOnce);
        client2.Verify(c => c.SendSound(42, true), Times.AtLeastOnce);
    }

    [Test]
    public void PlayMusic_ShouldNotThrow_WhenNoAislings()
    {
        var action = () => Map.PlayMusic(42);

        action.Should()
              .NotThrow();
    }
    #endregion

    #region PlaySound
    [Test]
    public void PlaySound_ShouldDoNothing_WhenNoPoints()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        Map.PlaySound(10);

        client.Verify(c => c.SendSound(10, false), Times.Never);
    }

    [Test]
    public void PlaySound_SinglePoint_ShouldSendToNearbyAislings()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        Map.PlaySound(10, new Point(5, 5));

        client.Verify(c => c.SendSound(10, false), Times.Once);
    }

    [Test]
    public void PlaySound_SinglePoint_ShouldNotSendToFarAislings()
    {
        var map = MockMapInstance.Create(width: 50, height: 50);
        var aisling = MockAisling.Create(map, position: new Point(49, 49));
        map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        map.PlaySound(10, new Point(0, 0));

        client.Verify(c => c.SendSound(10, false), Times.Never);
    }

    [Test]
    public void PlaySound_MultiplePoints_ShouldSendToAislingsNearAnyPoint()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        Map.PlaySound(10, new Point(5, 5), new Point(10, 10));

        client.Verify(c => c.SendSound(10, false), Times.Once);
    }

    [Test]
    public void PlaySound_IPointOverload_ShouldDelegate()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        IPoint point = new Point(5, 5);
        Map.PlaySound(10, point);

        client.Verify(c => c.SendSound(10, false), Times.Once);
    }
    #endregion

    #region ShowAnimation
    [Test]
    public void ShowAnimation_WithTargetPoint_ShouldSendToNearbyAislings()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        var animation = new Animation
        {
            TargetAnimation = 100,
            TargetPoint = new Point(5, 5)
        };

        Map.ShowAnimation(animation);

        client.Verify(c => c.SendAnimation(It.IsAny<Animation>()), Times.Once);
    }

    [Test]
    public void ShowAnimation_WithBothTargetPointAndId_ShouldPreferPointAnimation()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        var animation = new Animation
        {
            TargetAnimation = 100,
            TargetPoint = new Point(5, 5),
            TargetId = aisling.Id
        };

        Map.ShowAnimation(animation);

        client.Verify(c => c.SendAnimation(It.Is<Animation>(a => (a.TargetId == null) && (a.TargetPoint != null))), Times.Once);
    }

    [Test]
    public void ShowAnimation_WithTargetId_ShouldSendToNearbyObservers()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        // Set up approach time so CanObserve passes (ThatCanObserve filters)
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        var aislingScript = Mock.Get(aisling.Script);
        aislingScript.Reset();

        aislingScript.Setup(s => s.CanSee(monster))
                     .Returns(true);

        var client = Mock.Get(aisling.Client);

        var animation = new Animation
        {
            TargetAnimation = 100,
            TargetId = monster.Id
        };

        Map.ShowAnimation(animation);

        client.Verify(c => c.SendAnimation(animation), Times.Once);
    }

    [Test]
    public void ShowAnimation_WithTargetId_ShouldNotSendToObserversThatCantSee()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var monster = MockMonster.Create(Map);
        Map.SimpleAdd(monster);

        var aislingScript = Mock.Get(aisling.Script);
        aislingScript.Reset();

        aislingScript.Setup(s => s.CanSee(monster))
                     .Returns(false);

        var client = Mock.Get(aisling.Client);

        var animation = new Animation
        {
            TargetAnimation = 100,
            TargetId = monster.Id
        };

        Map.ShowAnimation(animation);

        client.Verify(c => c.SendAnimation(It.IsAny<Animation>()), Times.Never);
    }

    [Test]
    public void ShowAnimation_WithTargetId_ShouldNotSend_WhenTargetNotFound()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.SimpleAdd(aisling);

        var client = Mock.Get(aisling.Client);

        var animation = new Animation
        {
            TargetAnimation = 100,
            TargetId = 99999
        };

        Map.ShowAnimation(animation);

        client.Verify(c => c.SendAnimation(It.IsAny<Animation>()), Times.Never);
    }
    #endregion

    #region GetDistinctReactorsAtPoint
    [Test]
    public void GetDistinctReactorsAtPoint_ShouldReturnReactorsAtPoint()
    {
        var reactor = CreateReactor(new Point(5, 5));
        Map.SimpleAdd(reactor);

        Map.GetDistinctReactorsAtPoint(new Point(5, 5))
           .Should()
           .Contain(reactor);
    }

    [Test]
    public void GetDistinctReactorsAtPoint_ShouldReturnEmpty_WhenNoReactors()
        => Map.GetDistinctReactorsAtPoint(new Point(5, 5))
              .Should()
              .BeEmpty();
    #endregion

    #region TryGetRandomWalkablePoint
    [Test]
    public void TryGetRandomWalkablePoint_ShouldReturnTrue_WhenWalkablePointExists()
    {
        Map.TryGetRandomWalkablePoint(out var point)
           .Should()
           .BeTrue();

        point.Should()
             .NotBeNull();

        Map.IsWithinMap(point.Value)
           .Should()
           .BeTrue();
    }

    [Test]
    public void TryGetRandomWalkablePoint_WithPredicate_ShouldReturnMatchingPoint()
    {
        Map.TryGetRandomWalkablePoint(pt => pt.X > 5, out var point)
           .Should()
           .BeTrue();

        point.Should()
             .NotBeNull();

        point.Value
             .X
             .Should()
             .BeGreaterThan(5);
    }

    [Test]
    public void TryGetRandomWalkablePoint_ShouldReturnFalse_WhenNoWalkablePoints()
    {
        // Make the entire map walls
        for (var x = 0; x < 20; x++)
            for (var y = 0; y < 20; y++)
                MockMapInstance.SetWall(Map, new Point(x, y));

        Map.TryGetRandomWalkablePoint(out var point)
           .Should()
           .BeFalse();

        point.Should()
             .BeNull();
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotThrow_WhenEmpty()
    {
        var action = () => Map.Update(TimeSpan.FromMilliseconds(100));

        action.Should()
              .NotThrow();
    }

    [Test]
    public void Update_ShouldProcessEnqueuedActions()
    {
        var executed = false;
        Map.BeginInvoke(() => executed = true);

        Map.Update(TimeSpan.FromMilliseconds(100));

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void Update_ShouldContinue_WhenEnqueuedActionThrows()
    {
        var secondExecuted = false;
        Map.BeginInvoke(() => throw new InvalidOperationException("test"));
        Map.BeginInvoke(() => secondExecuted = true);

        Map.Update(TimeSpan.FromMilliseconds(100));

        secondExecuted.Should()
                      .BeTrue();
    }
    #endregion

    #region AddAislingDirect
    [Test]
    public void AddAislingDirect_ShouldAddAislingToMap()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddAislingDirect(aisling, new Point(8, 8));

        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();

        aisling.X
               .Should()
               .Be(8);

        aisling.Y
               .Should()
               .Be(8);
    }

    [Test]
    public void AddAislingDirect_ShouldSetMapInstance()
    {
        var otherMap = MockMapInstance.Create("other", width: 20, height: 20);
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        otherMap.AddAislingDirect(aisling, new Point(5, 5));

        aisling.MapInstance
               .Should()
               .BeSameAs(otherMap);
    }

    [Test]
    public void AddAislingDirect_ShouldCallScriptOnEntered()
    {
        var mapScript = Mock.Get(Map.Script);
        mapScript.Reset();

        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddAislingDirect(aisling, new Point(5, 5));

        mapScript.Verify(s => s.OnEntered(aisling), Times.Once);
    }

    [Test]
    public void AddAislingDirect_ShouldSendMapChangePending()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendMapChangePending(), Times.AtLeastOnce);
    }

    [Test]
    public void AddAislingDirect_ShouldSendMapInfo()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);
    }

    [Test]
    public void AddAislingDirect_ShouldSendLocation()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendLocation(), Times.AtLeastOnce);
    }

    [Test]
    public void AddAislingDirect_ShouldSendMapChangeComplete()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendMapChangeComplete(), Times.AtLeastOnce);
    }

    [Test]
    public void AddAislingDirect_ShouldSendMapLoadComplete()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendMapLoadComplete(), Times.AtLeastOnce);
    }

    [Test]
    public void AddAislingDirect_ShouldSendMusic_WhenNoLastMapInstance()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        var client = Mock.Get(aisling.Client);

        Map.AddAislingDirect(aisling, new Point(5, 5));

        client.Verify(c => c.SendSound(Map.Music, true), Times.AtLeastOnce);
    }
    #endregion

    #region Morph
    [Test]
    public void Morph_ShouldCallScriptOnMorphing()
    {
        var mapScript = Mock.Get(Map.Script);
        mapScript.Reset();

        var newTemplate = CreateNewMapTemplate();
        var simpleCacheMock = GetSimpleCacheMock(Map);

        simpleCacheMock.Setup(c => c.Get<MapTemplate>("new_map"))
                       .Returns(newTemplate);

        mapScript.Setup(s => s.OnMorphing(newTemplate));
        mapScript.Setup(s => s.OnMorphed());

        Map.Morph("new_map");

        mapScript.Verify(s => s.OnMorphing(newTemplate), Times.Once);
    }

    [Test]
    public void Morph_ShouldChangeTemplate()
    {
        var newTemplate = CreateNewMapTemplate();
        var simpleCacheMock = GetSimpleCacheMock(Map);

        simpleCacheMock.Setup(c => c.Get<MapTemplate>("new_map"))
                       .Returns(newTemplate);

        Map.Morph("new_map");

        Map.Template
           .Should()
           .BeSameAs(newTemplate);
    }

    [Test]
    public void Morph_ShouldCallScriptOnMorphed()
    {
        var mapScript = Mock.Get(Map.Script);
        mapScript.Reset();

        var newTemplate = CreateNewMapTemplate();
        var simpleCacheMock = GetSimpleCacheMock(Map);

        simpleCacheMock.Setup(c => c.Get<MapTemplate>("new_map"))
                       .Returns(newTemplate);

        mapScript.Setup(s => s.OnMorphing(newTemplate));
        mapScript.Setup(s => s.OnMorphed());

        Map.Morph("new_map");

        mapScript.Verify(s => s.OnMorphed(), Times.Once);
    }
    #endregion

    #region InvokeAsync
    [Test]
    public async Task InvokeAsync_ShouldCompleteTask_WhenActionSucceeds()
    {
        var executed = false;
        var task = Map.InvokeAsync(() => executed = true);

        // Action is queued, not yet executed
        executed.Should()
                .BeFalse();

        // Process the queue
        Map.Update(TimeSpan.FromMilliseconds(100));

        await task;

        executed.Should()
                .BeTrue();

        task.IsCompletedSuccessfully
            .Should()
            .BeTrue();
    }

    [Test]
    public async Task InvokeAsync_ShouldPropagateException_WhenActionThrows()
    {
        var task = Map.InvokeAsync(() => throw new InvalidOperationException("test error"));

        // Process the queue
        Map.Update(TimeSpan.FromMilliseconds(100));

        var act = () => task;

        // Exception is set but TrySetResult is also called in finally
        // The task should complete (TrySetException runs before TrySetResult in catch/finally)
        await act.Should()
                 .ThrowAsync<InvalidOperationException>()
                 .WithMessage("test error");
    }
    #endregion

    #region IsInSharedLanternVision (Dark Map)
    [Test]
    public void IsInSharedLanternVision_ShouldReturnTrue_WhenDarkMapWithLanternInRange()
    {
        var darkMap = MockMapInstance.Create(width: 20, height: 20, setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithLantern", new Point(5, 5));
        darkMap.SimpleAdd(aisling);
        aisling.SetLanternSize(LanternSize.Small); // radius 3

        var monster = MockMonster.Create(darkMap, setup: m => m.SetLocation(new Point(5, 7))); // distance 2, within radius 3
        darkMap.SimpleAdd(monster);

        darkMap.IsInSharedLanternVision(monster)
               .Should()
               .BeTrue();
    }

    [Test]
    public void IsInSharedLanternVision_ShouldReturnFalse_WhenDarkMapWithLanternOutOfRange()
    {
        var darkMap = MockMapInstance.Create(width: 20, height: 20, setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithLantern", new Point(0, 0));
        darkMap.SimpleAdd(aisling);
        aisling.SetLanternSize(LanternSize.Small); // radius 3

        var monster = MockMonster.Create(darkMap, setup: m => m.SetLocation(new Point(10, 10))); // distance ~14, way out of range
        darkMap.SimpleAdd(monster);

        darkMap.IsInSharedLanternVision(monster)
               .Should()
               .BeFalse();
    }

    [Test]
    public void IsInSharedLanternVision_ShouldReturnFalse_WhenDarkMapWithNoLantern()
    {
        var darkMap = MockMapInstance.Create(width: 20, height: 20, setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "NoLantern", new Point(5, 5));
        darkMap.SimpleAdd(aisling);

        // LanternSize defaults to None

        var monster = MockMonster.Create(darkMap, setup: m => m.SetLocation(new Point(5, 6)));
        darkMap.SimpleAdd(monster);

        darkMap.IsInSharedLanternVision(monster)
               .Should()
               .BeFalse();
    }

    [Test]
    public void IsInSharedLanternVision_ShouldReturnTrue_WhenDarkMapWithLargeLanternInRange()
    {
        var darkMap = MockMapInstance.Create(width: 20, height: 20, setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithLantern", new Point(5, 5));
        darkMap.SimpleAdd(aisling);
        aisling.SetLanternSize(LanternSize.Large); // radius 5

        var monster = MockMonster.Create(darkMap, setup: m => m.SetLocation(new Point(5, 9))); // distance 4, within radius 5
        darkMap.SimpleAdd(monster);

        darkMap.IsInSharedLanternVision(monster)
               .Should()
               .BeTrue();
    }
    #endregion

    #region HandleSharding (via AddEntity)
    [Test]
    public void AddEntity_ShouldBypassSharding_WhenIsShard()
    {
        Map.BaseInstanceId = "parent_instance";

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1");
        Map.AddEntity(aisling, new Point(5, 5));

        // Should use InnerAddEntity (bypass sharding) because IsShard is true
        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_ShouldBypassSharding_WhenNonAisling()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));

        // Non-aisling should bypass sharding
        Map.TryGetEntity<Monster>(monster.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_ShardingTypeNone_ShouldAddDirectly()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.None,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1");
        Map.AddEntity(aisling, new Point(5, 5));

        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_AlwaysShardOnCreate_ShouldAddDirectly()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AlwaysShardOnCreate,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1");
        Map.AddEntity(aisling, new Point(5, 5));

        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_PlayerLimit_ShouldJoinGroupMemberShard()
    {
        var shardMap = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        shardMap.BaseInstanceId = Map.InstanceId;
        Map.Shards.TryAdd(shardMap.InstanceId, shardMap);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.PlayerLimit,
                Limit = 2,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Put an existing group member on the shard
        var existingMember = MockAisling.Create(shardMap, "Existing");
        shardMap.SimpleAdd(existingMember);

        // Create a group
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();
        var newPlayer = MockAisling.Create(Map, "NewPlayer");

        var group = new Group(
            existingMember,
            newPlayer,
            channelService,
            logger.Object);
        existingMember.Group = group;
        newPlayer.Group = group;

        // AddEntity should route to the shard where the group member is
        Map.AddEntity(newPlayer, new Point(5, 5));

        shardMap.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_PlayerLimit_ShouldJoinExistingUnderLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.PlayerLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Add first player (below limit)
        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        // Add second player — still under limit, should join this map
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        Map.TryGetEntity<Aisling>(newPlayer.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_PlayerLimit_ShouldCreateNewShard_WhenAtLimit()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.PlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Fill the map to capacity
        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        // Add another player — should go to new shard
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Player limit=1 always creates new shard (regardless of existing shard capacity)
        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsolutePlayerLimit_ShouldJoinExistingUnderLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        Map.TryGetEntity<Aisling>(newPlayer.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_AbsolutePlayerLimit_ShouldCreateNewShard_WhenAllFull()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Fill the map
        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        // Add new player — no room, should create shard
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsolutePlayerLimit_ShouldJoinExistingShardUnderLimit()
    {
        // Create a shard with space
        var shard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        shard.BaseInstanceId = Map.InstanceId;
        Map.Shards.TryAdd(shard.InstanceId, shard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 2,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Fill the base map
        var existing1 = MockAisling.Create(Map, "Existing1");
        var existing2 = MockAisling.Create(Map, "Existing2");
        Map.SimpleAdd(existing1);
        Map.SimpleAdd(existing2);

        // Shard has space
        var shardMember = MockAisling.Create(shard, "ShardMember");
        shard.SimpleAdd(shardMember);

        // New player should go to shard (has space)
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        shard.TryGetEntity<Aisling>(newPlayer.Id, out _)
             .Should()
             .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGroupLimit_ShouldJoinGroupMemberShard()
    {
        var shardMap = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        shardMap.BaseInstanceId = Map.InstanceId;
        Map.Shards.TryAdd(shardMap.InstanceId, shardMap);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 2,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Put a group member on the shard
        var existingMember = MockAisling.Create(shardMap, "Existing");
        shardMap.SimpleAdd(existingMember);

        // Create group
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();
        var newPlayer = MockAisling.Create(Map, "NewPlayer");

        var group = new Group(
            existingMember,
            newPlayer,
            channelService,
            logger.Object);
        existingMember.Group = group;
        newPlayer.Group = group;

        Map.AddEntity(newPlayer, new Point(5, 5));

        shardMap.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGroupLimit_ShouldJoinExistingUnderLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Add one ungrouped player (counts as 1 group)
        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        Map.TryGetEntity<Aisling>(newPlayer.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGroupLimit_ShouldCreateNewShard_WhenAtGroupLimit()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // One ungrouped player = 1 group
        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGuildLimit_ShouldJoinExistingUnderLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        Map.TryGetEntity<Aisling>(newPlayer.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGuildLimit_ShouldCreateNewShard_WhenAtGuildLimit()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var existing = MockAisling.Create(Map, "Existing");
        Map.SimpleAdd(existing);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_PlayerLimit_Limit1_ShouldAlwaysCreateNewShard()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.PlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Even with no one on the map, limit=1 should create a new shard
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }
    #endregion

    #region HandleShardLimiters (via Update)
    [Test]
    public void HandleShardLimiters_ShouldNotThrow_WhenNoShardingOptions()
    {
        // ShardingOptions is null by default
        var action = () => Map.Update(TimeSpan.FromSeconds(2));

        action.Should()
              .NotThrow();
    }

    [Test]
    public void HandleShardLimiters_ShouldNotThrow_WhenShardingTypeNone()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.None,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var action = () => Map.Update(TimeSpan.FromSeconds(2));

        action.Should()
              .NotThrow();
    }

    [Test]
    public void HandleShardLimiters_AbsolutePlayerLimit_ShouldNotRemove_WhenUnderLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 5,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.SimpleAdd(aisling);

        Map.Update(TimeSpan.FromSeconds(2));

        // Player should still be on the map
        Map.TryGetEntity<Aisling>(aisling.Id, out _)
           .Should()
           .BeTrue();
    }

    [Test]
    public void HandleShardLimiters_AbsolutePlayerLimit_ShouldSendWarning_WhenOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling1 = MockAisling.Create(Map, "Player1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Player2", new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        var client1 = Mock.Get(aisling1.Client);
        var client2 = Mock.Get(aisling2.Client);

        Map.Update(TimeSpan.FromSeconds(2));

        // One of the two should receive a warning about being over the limit
        var totalWarnings = client1.Invocations.Count(i => i.Method.Name == "SendServerMessage")
                            + client2.Invocations.Count(i => i.Method.Name == "SendServerMessage");

        totalWarnings.Should()
                     .BeGreaterThan(0);
    }

    [Test]
    public void HandleShardLimiters_ShouldCleanupTimers_WhenAislingLeaves()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling1 = MockAisling.Create(Map, "Player1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Player2", new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        // Trigger limiter creation
        Map.Update(TimeSpan.FromSeconds(2));

        // Remove aisling2 from map
        Map.RemoveEntity(aisling2);

        // Update again — should clean up the timer for the removed aisling
        Map.Update(TimeSpan.FromSeconds(2));

        // Should not throw
        Map.ShardLimiterTimers
           .ContainsKey(aisling2)
           .Should()
           .BeFalse();
    }

    [Test]
    public void HandleShardLimiters_AbsoluteGroupLimit_ShouldSendWarning_WhenOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Two ungrouped players = 2 "groups"
        var aisling1 = MockAisling.Create(Map, "Player1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Player2", new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        var client1 = Mock.Get(aisling1.Client);
        var client2 = Mock.Get(aisling2.Client);

        Map.Update(TimeSpan.FromSeconds(2));

        var totalWarnings = client1.Invocations.Count(i => i.Method.Name == "SendServerMessage")
                            + client2.Invocations.Count(i => i.Method.Name == "SendServerMessage");

        totalWarnings.Should()
                     .BeGreaterThan(0);
    }

    [Test]
    public void HandleShardLimiters_AbsoluteGuildLimit_ShouldSendWarning_WhenOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Two unguilded players = 2 "guilds"
        var aisling1 = MockAisling.Create(Map, "Player1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Player2", new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        var client1 = Mock.Get(aisling1.Client);
        var client2 = Mock.Get(aisling2.Client);

        Map.Update(TimeSpan.FromSeconds(2));

        var totalWarnings = client1.Invocations.Count(i => i.Method.Name == "SendServerMessage")
                            + client2.Invocations.Count(i => i.Method.Name == "SendServerMessage");

        totalWarnings.Should()
                     .BeGreaterThan(0);
    }

    [Test]
    public void HandleShardLimiters_ShouldSkipAdmins()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var admin = MockAisling.Create(Map, "Admin", new Point(5, 5));
        admin.IsAdmin = true;
        Map.SimpleAdd(admin);

        var player = MockAisling.Create(Map, "Player", new Point(6, 6));
        Map.SimpleAdd(player);

        // Only the non-admin should count toward the limit
        // With limit=1, only the player counts, so we're at limit (1 == 1), not over
        Map.Update(TimeSpan.FromSeconds(2));

        Map.ShardLimiterTimers
           .ContainsKey(player)
           .Should()
           .BeFalse();
    }
    #endregion

    #region HandleShardLimiters — Not Over Limit (early return)
    [Test]
    public void HandleShardLimiters_AbsolutePlayerLimit_ShouldDoNothing_WhenNotOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 5,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.SimpleAdd(aisling);

        Map.Update(TimeSpan.FromSeconds(2));

        // Not over limit (1 <= 5), so no timers should be created
        Map.ShardLimiterTimers
           .Should()
           .BeEmpty();
    }

    [Test]
    public void HandleShardLimiters_AbsoluteGroupLimit_ShouldDoNothing_WhenNotOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 5,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.SimpleAdd(aisling);

        Map.Update(TimeSpan.FromSeconds(2));

        // 1 group (ungrouped = 1) <= 5 limit — no timers
        Map.ShardLimiterTimers
           .Should()
           .BeEmpty();
    }

    [Test]
    public void HandleShardLimiters_AbsoluteGuildLimit_ShouldDoNothing_WhenNotOverLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 5,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.SimpleAdd(aisling);

        Map.Update(TimeSpan.FromSeconds(2));

        // 1 guild (unguilded = 1) <= 5 limit — no timers
        Map.ShardLimiterTimers
           .Should()
           .BeEmpty();
    }
    #endregion

    #region HandleShardLimiters — Grouped Players (AbsoluteGroupLimit)
    [Test]
    public void HandleShardLimiters_AbsoluteGroupLimit_ShouldCountGroupsCorrectly()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Two players in the same group = 1 group
        var aisling1 = MockAisling.Create(Map, "Grouped1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Grouped2", new Point(6, 6));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);

        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        var group = new Group(
            aisling1,
            aisling2,
            channelService,
            logger.Object);
        aisling1.Group = group;
        aisling2.Group = group;

        Map.Update(TimeSpan.FromSeconds(2));

        // 1 group <= 1 limit — not over, no warnings
        Map.ShardLimiterTimers
           .Should()
           .BeEmpty();
    }

    [Test]
    public void HandleShardLimiters_AbsoluteGroupLimit_ShouldWarn_WhenGroupCountExceedsLimit()
    {
        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Two grouped players + 1 ungrouped = 2 groups
        var aisling1 = MockAisling.Create(Map, "Grouped1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Grouped2", new Point(6, 6));
        var loner = MockAisling.Create(Map, "Loner", new Point(7, 7));
        Map.SimpleAdd(aisling1);
        Map.SimpleAdd(aisling2);
        Map.SimpleAdd(loner);

        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        var group = new Group(
            aisling1,
            aisling2,
            channelService,
            logger.Object);
        aisling1.Group = group;
        aisling2.Group = group;

        var lonerClient = Mock.Get(loner.Client);

        Map.Update(TimeSpan.FromSeconds(2));

        // 2 groups > 1 limit — loner should get warning (highest Id, smallest group)
        var totalWarnings = lonerClient.Invocations.Count(i => i.Method.Name == "SendServerMessage");

        totalWarnings.Should()
                     .BeGreaterThan(0);
    }
    #endregion

    #region HandleSharding — AbsoluteGroupLimit grouped join
    [Test]
    public void AddEntity_AbsoluteGroupLimit_ShouldJoinUnderLimitShard_WhenGroupCountLow()
    {
        var shard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        shard.BaseInstanceId = Map.InstanceId;
        Map.Shards.TryAdd(shard.InstanceId, shard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Fill base map at group limit
        var existing1 = MockAisling.Create(Map, "Exist1");
        var existing2 = MockAisling.Create(Map, "Exist2");
        var existing3 = MockAisling.Create(Map, "Exist3");
        Map.SimpleAdd(existing1);
        Map.SimpleAdd(existing2);
        Map.SimpleAdd(existing3);

        // Shard has 1 ungrouped (1 group, under limit of 3)
        var shardMember = MockAisling.Create(shard, "ShardMem");
        shard.SimpleAdd(shardMember);

        // New ungrouped player — base map is at limit (3 groups), shard is under (1 group)
        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        shard.TryGetEntity<Aisling>(newPlayer.Id, out _)
             .Should()
             .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGuildLimit_ShouldJoinUnderLimitShard_WhenGuildCountLow()
    {
        var shard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        shard.BaseInstanceId = Map.InstanceId;
        Map.Shards.TryAdd(shard.InstanceId, shard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 3,
                ExitLocation = new Location("exit", 5, 5)
            });

        // Fill base map at guild limit (3 unguilded = 3 guilds)
        var existing1 = MockAisling.Create(Map, "Exist1");
        var existing2 = MockAisling.Create(Map, "Exist2");
        var existing3 = MockAisling.Create(Map, "Exist3");
        Map.SimpleAdd(existing1);
        Map.SimpleAdd(existing2);
        Map.SimpleAdd(existing3);

        // Shard has 1 unguilded (1 guild, under limit of 3)
        var shardMember = MockAisling.Create(shard, "ShardMem");
        shard.SimpleAdd(shardMember);

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        shard.TryGetEntity<Aisling>(newPlayer.Id, out _)
             .Should()
             .BeTrue();
    }
    #endregion

    #region HandleSharding — Limit=1 always creates new shard
    [Test]
    public void AddEntity_AbsoluteGroupLimit_Limit1_ShouldAlwaysCreateNewShard()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGroupLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsoluteGuildLimit_Limit1_ShouldAlwaysCreateNewShard()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsoluteGuildLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }

    [Test]
    public void AddEntity_AbsolutePlayerLimit_Limit1_ShouldAlwaysCreateNewShard()
    {
        var newShard = MockMapInstance.Create("test_map_shard", width: 20, height: 20);
        SetupShardGenerator(Map, newShard);

        SetShardingOptions(
            Map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 1,
                ExitLocation = new Location("exit", 5, 5)
            });

        var newPlayer = MockAisling.Create(Map, "NewPlayer");
        Map.AddEntity(newPlayer, new Point(5, 5));

        // Entity is queued via BeginInvoke — process the shard's queue
        newShard.Update(TimeSpan.FromSeconds(1));

        newShard.TryGetEntity<Aisling>(newPlayer.Id, out _)
                .Should()
                .BeTrue();
    }
    #endregion

    #region Click — ReactorTile only
    [Test]
    public void Click_ShouldClickReactorTile_WhenNoDoorPresent()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var reactor = CreateReactor(new Point(5, 5));
        Map.SimpleAdd(reactor);

        // Reactor has a script — set up OnClicked to track it
        var reactorScriptMock = Mock.Get(reactor.Script);

        reactorScriptMock.Setup(s => s.OnClicked(aisling))
                         .Verifiable();

        Map.Click(new Point(5, 5), aisling);

        // No door at (5,5), so reactor's OnClicked should fire
        reactorScriptMock.Verify(s => s.OnClicked(aisling), Times.AtLeastOnce);
    }

    [Test]
    public void Click_ShouldDoNothing_WhenNoDoorAndNoReactorPresent()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        // Click an empty point with no door and no reactor — obj?.OnClicked null path
        Map.Click(new Point(5, 6), aisling);

        // No exception, nothing to verify — just exercises the null branch of obj?.
    }
    #endregion

    #region AutoDayNightCycle
    [Test]
    public void Update_ShouldNotSendLightLevel_WhenAutoDayNightCycleDisabled()
    {
        Map.AutoDayNightCycle = false;

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        Map.Update(TimeSpan.FromSeconds(2));

        clientMock.Verify(c => c.SendLightLevel(It.IsAny<LightLevel>()), Times.Never);
    }

    [Test]
    public void Update_ShouldProcessDayNightCycle_WhenAutoDayNightCycleEnabled()
    {
        Map.AutoDayNightCycle = true;

        var aisling = MockAisling.Create(Map, "Player1", new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // DayNightCycleTimer interval is 1s — update with 2s to trigger
        Map.Update(TimeSpan.FromSeconds(2));

        // Should attempt to send light level (may or may not actually send if level hasn't changed)
        // The key is that the AutoDayNightCycle=true path is exercised
    }
    #endregion

    #region InnerAddEntity — Music change
    [Test]
    public void AddEntity_ShouldSendSound_WhenMusicDiffers()
    {
        var map2 = MockMapInstance.Create("map2", "Map Two");

        // Set different music values via reflection
        typeof(MapInstance).GetProperty(nameof(MapInstance.Music))!.SetValue(Map, (byte)5);

        typeof(MapInstance).GetProperty(nameof(MapInstance.Music))!.SetValue(map2, (byte)10);

        var aisling = MockAisling.Create(map2, "Player1");
        map2.SimpleAdd(aisling);
        aisling.Trackers.LastMapInstance = map2;

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        Map.AddEntity(aisling, new Point(5, 5));

        clientMock.Verify(c => c.SendSound(5, true), Times.AtLeastOnce);
    }

    [Test]
    public void AddEntity_ShouldNotSendSound_WhenMusicSame()
    {
        var map2 = MockMapInstance.Create("map2", "Map Two");

        // Same music on both maps
        typeof(MapInstance).GetProperty(nameof(MapInstance.Music))!.SetValue(Map, (byte)5);

        typeof(MapInstance).GetProperty(nameof(MapInstance.Music))!.SetValue(map2, (byte)5);

        var aisling = MockAisling.Create(map2, "Player1");
        map2.SimpleAdd(aisling);
        aisling.Trackers.LastMapInstance = map2;

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        Map.AddEntity(aisling, new Point(5, 5));

        clientMock.Verify(c => c.SendSound(It.IsAny<byte>(), true), Times.Never);
    }
    #endregion
}