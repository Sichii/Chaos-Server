using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Models.Data;

public sealed class MonsterSpawn : IDeltaUpdatable
{
    public required int AggroRange { get; init; }
    public required ICollection<IPoint> BlackList { get; init; }
    public required int ExpReward { get; init; }
    public required ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();
    public required LootTable? LootTable { get; set; }
    public MapInstance MapInstance { get; set; } = null!;
    public required int MaxAmount { get; init; }
    public required int MaxGoldDrop { get; init; }
    public required int MaxPerSpawn { get; init; }
    public required int MinGoldDrop { get; init; }
    public required IMonsterFactory MonsterFactory { get; init; }
    public required MonsterTemplate MonsterTemplate { get; init; }
    public Rectangle? SpawnArea { get; set; }
    public required IIntervalTimer SpawnTimer { get; init; }

    public void FullSpawn()
    {
        while (true)
        {
            var currentCount = MapInstance.GetEntities<Monster>()
                                          .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

            if (currentCount >= MaxAmount)
                break;

            SpawnMonsters();
        }
    }

    private void GenerateGoldAndExp(Monster monster)
    {
        monster.Gold = Random.Shared.Next(MinGoldDrop, MaxGoldDrop + 1);
        monster.Experience = ExpReward;
    }

    private IPoint GenerateSpawnPoint(ICollection<IPoint> blackList) => MapInstance.Template.Bounds.GetRandomPoint(
        pt => MapInstance.IsWalkable(pt, MonsterTemplate.Type) && !blackList.Contains(pt, PointEqualityComparer.Instance));

    private void SpawnMonsters()
    {
        var currentCount = MapInstance.GetEntities<Monster>()
                                      .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

        if (currentCount >= MaxAmount)
            return;

        var spawnAmount = Math.Min(MaxAmount - currentCount, MaxPerSpawn);
        var monsters = new List<Monster>();

        for (var i = 0; i < spawnAmount; i++)
        {
            var point = GenerateSpawnPoint(BlackList);

            var monster = MonsterFactory.Create(
                MonsterTemplate.TemplateKey,
                MapInstance,
                point,
                ExtraScriptKeys);

            monster.AggroRange = AggroRange;
            monster.LootTable = LootTable;

            GenerateGoldAndExp(monster);
            monsters.Add(monster);
        }

        MapInstance.AddObjects(monsters);

        foreach (var monster in monsters)
            monster.Script.OnSpawn();
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
            SpawnMonsters();
    }
}