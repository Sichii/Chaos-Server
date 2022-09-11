using Chaos.Containers;
using Chaos.Data;
using Chaos.Entities.Schemas.Content;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;
using Chaos.Time;

namespace Chaos.Services.Mappers;

public class MonsterSpawnMapperProfile : IMapperProfile<MonsterSpawn, MonsterSpawnSchema>
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    public MonsterSpawnMapperProfile(IMonsterFactory monsterFactory, ISimpleCache simpleCache)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public MonsterSpawn Map(MonsterSpawnSchema obj) => new()
    {
        AggroRange = obj.AggroRange ?? 0,
        ExpReward = obj.ExpReward,
        LootTable = obj.LootTableKey == null ? null : SimpleCache.GetObject<LootTable>(obj.LootTableKey),
        MaxAmount = obj.MaxAmount,
        MaxGoldDrop = obj.MaxGoldDrop,
        MinGoldDrop = obj.MinGoldDrop,
        MonsterFactory = MonsterFactory,
        MonsterTemplate = SimpleCache.GetObject<MonsterTemplate>(obj.MonsterTemplateKey),
        MaxPerSpawn = obj.MaxPerSpawn,
        ExtraScriptKeys = new HashSet<string>(obj.ExtraScriptKeys, StringComparer.OrdinalIgnoreCase),
        SpawnArea = obj.SpawnArea!,
        SpawnTimer = obj.IntervalVariancePct.HasValue
            ? new RandomizedIntervalTimer(TimeSpan.FromSeconds(obj.IntervalSecs), obj.IntervalVariancePct.Value)
            : new IntervalTimer(TimeSpan.FromSeconds(obj.IntervalSecs))
    };

    /// <inheritdoc />
    public MonsterSpawnSchema Map(MonsterSpawn obj) => throw new NotImplementedException();
}