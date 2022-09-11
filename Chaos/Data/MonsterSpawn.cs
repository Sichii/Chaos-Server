using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Templates;
using Chaos.Time.Abstractions;

namespace Chaos.Data;

public class MonsterSpawn : IDeltaUpdatable
{
    public MapInstance MapInstance { get; set; } = null!;
    public required LootTable? LootTable { get; init; }
    public required IIntervalTimer SpawnTimer { get; init; }
    public required int MaxPerSpawn { get; init; }
    public required int MaxAmount { get; init; }
    public required int AggroRange { get; init; }
    public required int MinGoldDrop { get; init; }
    public required int MaxGoldDrop { get; init; }
    public required int ExpReward { get; init; }
    public required ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();
    public required IMonsterFactory MonsterFactory { get; init; }
    public required MonsterTemplate MonsterTemplate { get; init; }
    public Rectangle? SpawnArea { get; set; }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
            SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        var currentCount = MapInstance.AllObjects<Monster>()
                                      .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

        if (currentCount >= MaxAmount)
            return;

        var spawnAmount = Math.Min(MaxAmount - currentCount, MaxPerSpawn);
        var monsters = new List<Monster>();
        
        for (var i = 0; i < spawnAmount; i++)
        {
            var point = GenerateSpawnPoint();

            var monster = MonsterFactory.Create(
                MonsterTemplate.TemplateKey,
                MapInstance,
                point,
                ExtraScriptKeys);

            monster.AggroRange = AggroRange;
            
            GenerateRewards(monster);
            monsters.Add(monster);
        }
        
        MapInstance.AddObjects(monsters);
    }

    private Point GenerateSpawnPoint()
    {
        Point point;

        do
            point = SpawnArea!.RandomPoint();
        while (!MapInstance.IsWalkable(point, MonsterTemplate.Type == CreatureType.WalkThrough));

        return point;
    }

    private void GenerateRewards(Monster monster)
    {
        if (LootTable != null)
            monster.Items.AddRange(LootTable.GenerateLoot());
        
        monster.Gold = Random.Shared.Next(MinGoldDrop, MaxGoldDrop + 1);
        monster.Experience = ExpReward;
    }
}