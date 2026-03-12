#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Models.Data;

public sealed class MonsterSpawn : IDeltaUpdatable
{
    public required ICollection<IPoint> BlackList { get; init; }
    public required Direction? Direction { get; init; }
    public required ICollection<LootTable> ExtraLootTables { get; set; } = [];
    public required ICollection<string> ExtraScriptKeys { get; init; } = [];
    public ILootTable? FinalLootTable { get; set; }
    public MapInstance MapInstance { get; set; } = null!;
    public required int MaxAmount { get; init; }
    public required int MaxPerSpawn { get; init; }
    public required IMonsterFactory MonsterFactory { get; init; }
    public required MonsterTemplate MonsterTemplate { get; init; }
    public required bool OnlyCountMonstersInSpawnArea { get; set; }
    public required Rectangle? SpawnArea { get; set; }
    public required IIntervalTimer SpawnTimer { get; init; }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
            SpawnMonsters();
    }

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

    private int GetCurrentCount()
        => SpawnArea is not null && OnlyCountMonstersInSpawnArea
            ? MapInstance.GetEntitiesWithin<Monster>(SpawnArea)
                         .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey))
            : MapInstance.GetEntities<Monster>()
                         .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

    private bool PointValidator(Monster monster, Point point)
        => (SpawnArea is null || SpawnArea.ContainsPoint(point)) && !BlackList.Contains(point, PointEqualityComparer.Instance);

    private void SpawnMonsters()
    {
        var currentCount = GetCurrentCount();

        if (currentCount >= MaxAmount)
            return;

        var spawnAmount = Math.Min(MaxAmount - currentCount, MaxPerSpawn);
        var monsters = new List<Monster>();
        var defaultPoint = new Point();

        for (var i = 0; i < spawnAmount; i++)
        {
            var monster = MonsterFactory.Create(
                MonsterTemplate.TemplateKey,
                MapInstance,
                defaultPoint,
                ExtraScriptKeys);

            if (!TryGenerateSpawnPoint(monster, out var spawnPoint))
                continue;

            FinalLootTable ??= ExtraLootTables.Count != 0
                ? new CompositeLootTable(ExtraLootTables.Append(monster.LootTable))
                : monster.LootTable;

            monster.SetLocation(MapInstance, spawnPoint);
            monster.LootTable = FinalLootTable;
            monster.Direction = Direction ?? (Direction)Random.Shared.Next(4);
            monsters.Add(monster);
        }

        MapInstance.AddEntities(monsters);

        foreach (var monster in monsters)
            monster.Script.OnSpawn();
    }

    private bool TryGenerateSpawnPoint(Monster monster, [NotNullWhen(true)] out Point? spawnPoint)
    {
        spawnPoint = null;

        if (MapInstance.TryGetRandomWalkablePoint(pt => PointValidator(monster, pt), out spawnPoint))
            return true;

        return false;
    }
}