using Chaos.Containers;
using Chaos.Data;
using Chaos.Factories.Abstractions;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class MonsterMapperProfile : IMapperProfile<MonsterSpawn, MonsterSpawnSchema>,
                                           IMapperProfile<MonsterTemplate, MonsterTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;

    public MonsterMapperProfile(IMonsterFactory monsterFactory, ISimpleCache simpleCache, ITypeMapper mapper)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        Mapper = mapper;
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

    /// <inheritdoc />
    public MonsterTemplate Map(MonsterTemplateSchema obj)
    {
        obj.StatSheet.CurrentHp = obj.StatSheet.MaximumHp;
        obj.StatSheet.CurrentMp = obj.StatSheet.MaximumMp;

        return new MonsterTemplate
        {
            AssailIntervalMs = obj.AssailIntervalMs,
            SkillIntervalMs = obj.SkillIntervalMs,
            SpellIntervalMs = obj.SpellIntervalMs,
            Direction = obj.Direction,
            MoveIntervalMs = obj.MoveIntervalMs,
            Name = obj.Name,
            ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
            SkillTemplateKeys = new HashSet<string>(obj.SkillTemplateKeys, StringComparer.OrdinalIgnoreCase),
            SpellTemplateKeys = new HashSet<string>(obj.SpellTemplateKeys, StringComparer.OrdinalIgnoreCase),
            Sprite = obj.Sprite,
            StatSheet = Mapper.Map<StatSheet>(obj.StatSheet),
            TemplateKey = obj.TemplateKey,
            Type = obj.Type,
            WanderIntervalMs = obj.WanderIntervalMs,
            ScriptVars = obj.ScriptVars
        };
    }

    /// <inheritdoc />
    public MonsterTemplateSchema Map(MonsterTemplate obj) => throw new NotImplementedException();
}