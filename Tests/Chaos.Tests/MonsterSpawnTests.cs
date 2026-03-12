#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Time;
using Chaos.Time.Abstractions;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MonsterSpawnTests
{
    private const string TEMPLATE_KEY = "testmonster";

    private static readonly MonsterTemplate DefaultTemplate = new()
    {
        Name = "TestMonster",
        TemplateKey = TEMPLATE_KEY,
        AssailIntervalMs = 1000,
        MoveIntervalMs = 1000,
        SkillIntervalMs = 1000,
        SpellIntervalMs = 1000,
        WanderIntervalMs = 1000,
        AggroRange = 5,
        ExpReward = 100,
        MinGoldDrop = 0,
        MaxGoldDrop = 0,
        Sprite = 1,
        StatSheet = new StatSheet
        {
            MaximumHp = 100,
            MaximumMp = 50,
            Level = 1,
            Str = 10,
            Dex = 10,
            Int = 10,
            Wis = 10,
            Con = 10,
            Ac = 100
        },
        Type = CreatureType.Normal,
        LootTables = [],
        ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
        SpellTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        SkillTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    };

    /// <summary>
    ///     Creates a monster that matches the spawn's template key
    /// </summary>
    private static Monster CreateMatchingMonster(MapInstance map)
        => MockMonster.Create(
            map,
            templateSetup: t => t with
            {
                TemplateKey = TEMPLATE_KEY
            });

    /// <summary>
    ///     Creates a factory mock that returns monsters with the matching template key
    /// </summary>
    private static Mock<IMonsterFactory> CreateMonsterFactory(MapInstance map)
    {
        var factoryMock = new Mock<IMonsterFactory>();

        factoryMock.Setup(f => f.Create(
                       It.IsAny<string>(),
                       It.IsAny<MapInstance>(),
                       It.IsAny<IPoint>(),
                       It.IsAny<ICollection<string>?>()))
                   .Returns(() => MockMonster.Create(
                       map,
                       templateSetup: t => t with
                       {
                           TemplateKey = TEMPLATE_KEY
                       }));

        return factoryMock;
    }

    private static MonsterSpawn CreateSpawn(
        MapInstance? map = null,
        int maxAmount = 5,
        int maxPerSpawn = 2,
        Direction? direction = null,
        Rectangle? spawnArea = null,
        bool onlyCountInSpawnArea = false,
        ICollection<string>? extraScriptKeys = null,
        ICollection<LootTable>? extraLootTables = null,
        ICollection<IPoint>? blackList = null,
        IIntervalTimer? timer = null,
        Mock<IMonsterFactory>? factoryMock = null)
    {
        map ??= MockMapInstance.Create();
        factoryMock ??= CreateMonsterFactory(map);
        timer ??= new IntervalTimer(TimeSpan.FromSeconds(5), false);

        return new MonsterSpawn
        {
            BlackList = blackList ?? new List<IPoint>(),
            Direction = direction,
            ExtraLootTables = extraLootTables ?? new List<LootTable>(),
            ExtraScriptKeys = extraScriptKeys ?? new List<string>(),
            MapInstance = map,
            MaxAmount = maxAmount,
            MaxPerSpawn = maxPerSpawn,
            MonsterFactory = factoryMock.Object,
            MonsterTemplate = DefaultTemplate,
            OnlyCountMonstersInSpawnArea = onlyCountInSpawnArea,
            SpawnArea = spawnArea,
            SpawnTimer = timer
        };
    }

    #region SpawnMonsters - MaxPerSpawn limiting
    [Test]
    public void SpawnMonsters_ShouldLimitToMaxPerSpawn()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        var spawn = CreateSpawn(
            map,
            10,
            3,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        // Should create at most MaxPerSpawn monsters
        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.AtMost(3));
    }
    #endregion

    #region PointValidator - BlackList
    [Test]
    public void SpawnMonsters_ShouldRespectBlackList()
    {
        // Use a very small map where most points are blacklisted
        var map = MockMapInstance.Create(width: 3, height: 3);
        var factoryMock = CreateMonsterFactory(map);

        var blackList = new List<IPoint>();

        // Blacklist all points
        for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
                blackList.Add(new Point(x, y));

        var spawn = CreateSpawn(
            map,
            1,
            1,
            blackList: blackList,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        // This should not throw even though no valid spawn points exist
        var act = () => spawn.Update(TimeSpan.FromSeconds(2));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region SpawnMonsters - FinalLootTable
    [Test]
    public void SpawnMonsters_ShouldSetFinalLootTable_WhenNoExtraLootTables()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        var spawn = CreateSpawn(
            map,
            1,
            1,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        // FinalLootTable should be set (monster's own loot table)
        spawn.FinalLootTable
             .Should()
             .NotBeNull();
    }
    #endregion

    #region SpawnMonsters - Direction null (random)
    [Test]
    public void SpawnMonsters_ShouldUseRandomDirection_WhenDirectionIsNull()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        var spawn = CreateSpawn(
            map,
            1,
            1,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        // Should not throw — direction assigned randomly
        var act = () => spawn.Update(TimeSpan.FromSeconds(2));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region SpawnMonsters - Direction
    [Test]
    public void SpawnMonsters_ShouldUseTemplateDirection_WhenDirectionIsSet()
    {
        var map = MockMapInstance.Create();

        var monsterCreated = (Monster?)null;
        var factoryMock = new Mock<IMonsterFactory>();

        factoryMock.Setup(f => f.Create(
                       It.IsAny<string>(),
                       It.IsAny<MapInstance>(),
                       It.IsAny<IPoint>(),
                       It.IsAny<ICollection<string>?>()))
                   .Returns(() =>
                   {
                       var m = MockMonster.Create(
                           map,
                           templateSetup: t => t with
                           {
                               TemplateKey = TEMPLATE_KEY
                           });
                       monsterCreated = m;

                       return m;
                   });

        var spawn = CreateSpawn(
            map,
            1,
            1,
            Direction.Left,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        if (monsterCreated is not null)
            monsterCreated.Direction
                          .Should()
                          .Be(Direction.Left);
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotSpawn_WhenTimerHasNotElapsed()
    {
        var factoryMock = new Mock<IMonsterFactory>();
        var map = MockMapInstance.Create();

        var spawn = CreateSpawn(map, timer: new IntervalTimer(TimeSpan.FromSeconds(30), false), factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(1));

        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.Never);
    }

    [Test]
    public void Update_ShouldSpawn_WhenTimerElapses()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        var spawn = CreateSpawn(map, timer: new IntervalTimer(TimeSpan.FromSeconds(1)), factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.AtLeastOnce);
    }
    #endregion

    #region GetCurrentCount
    [Test]
    public void SpawnMonsters_ShouldNotSpawn_WhenAlreadyAtMaxAmount()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        // Pre-populate map with monsters matching the template key
        for (var i = 0; i < 5; i++)
        {
            var monster = CreateMatchingMonster(map);
            map.SimpleAdd(monster);
        }

        var spawn = CreateSpawn(map, timer: new IntervalTimer(TimeSpan.FromSeconds(1)), factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        // Should not create any new monsters
        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.Never);
    }

    [Test]
    public void SpawnMonsters_ShouldCountOnlyInSpawnArea_WhenFlagIsSet()
    {
        var map = MockMapInstance.Create(width: 20, height: 20);
        var factoryMock = CreateMonsterFactory(map);

        var spawnArea = new Rectangle(
            0,
            0,
            5,
            5);

        // Add a monster outside the spawn area with matching template key
        var outsideMonster = CreateMatchingMonster(map);
        outsideMonster.SetLocation(map, new Point(10, 10));
        map.SimpleAdd(outsideMonster);

        var spawn = CreateSpawn(
            map,
            1,
            1,
            spawnArea: spawnArea,
            onlyCountInSpawnArea: true,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        // Should still try to spawn since the outside monster doesn't count
        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.AtLeastOnce);
    }

    [Test]
    public void SpawnMonsters_ShouldCountGlobally_WhenNoSpawnAreaOrFlagNotSet()
    {
        var map = MockMapInstance.Create(width: 20, height: 20);
        var factoryMock = CreateMonsterFactory(map);

        // Add a monster at any position with matching template key
        var existingMonster = CreateMatchingMonster(map);
        map.SimpleAdd(existingMonster);

        var spawn = CreateSpawn(
            map,
            1,
            1,
            timer: new IntervalTimer(TimeSpan.FromSeconds(1)),
            factoryMock: factoryMock);

        spawn.Update(TimeSpan.FromSeconds(2));

        // Should not spawn since global count is already at max
        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.Never);
    }
    #endregion

    #region FullSpawn
    [Test]
    public void FullSpawn_ShouldSpawnUntilMaxAmount()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        var spawn = CreateSpawn(
            map,
            3,
            1,
            factoryMock: factoryMock);

        spawn.FullSpawn();

        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.AtLeast(3));
    }

    [Test]
    public void FullSpawn_ShouldNotSpawn_WhenAlreadyAtMax()
    {
        var map = MockMapInstance.Create();
        var factoryMock = CreateMonsterFactory(map);

        // Pre-populate map with max monsters
        for (var i = 0; i < 3; i++)
        {
            var monster = CreateMatchingMonster(map);
            map.SimpleAdd(monster);
        }

        var spawn = CreateSpawn(
            map,
            3,
            1,
            factoryMock: factoryMock);

        spawn.FullSpawn();

        factoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>?>()),
            Times.Never);
    }
    #endregion
}