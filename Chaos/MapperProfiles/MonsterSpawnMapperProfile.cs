using Chaos.Containers;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class MonsterSpawnMapperProfile : IMapperProfile<MonsterSpawn, MonsterSpawnSchema>
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
        AggroRange = obj.AggroRange,
        ExpReward = obj.ExpReward,
        LootTable = obj.LootTableKey == null ? null : SimpleCache.Get<LootTable>(obj.LootTableKey),
        MaxAmount = obj.MaxAmount,
        MaxGoldDrop = obj.MaxGoldDrop,
        MinGoldDrop = obj.MinGoldDrop,
        MonsterFactory = MonsterFactory,
        MonsterTemplate = SimpleCache.Get<MonsterTemplate>(obj.MonsterTemplateKey),
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