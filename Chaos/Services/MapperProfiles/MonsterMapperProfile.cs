using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

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
        LootTable = obj.LootTableKey == null ? null : SimpleCache.Get<LootTable>(obj.LootTableKey),
        MaxAmount = obj.MaxAmount,
        MonsterFactory = MonsterFactory,
        MonsterTemplate = SimpleCache.Get<MonsterTemplate>(obj.MonsterTemplateKey),
        MaxPerSpawn = obj.MaxPerSpawn,
        ExtraScriptKeys = new HashSet<string>(obj.ExtraScriptKeys, StringComparer.OrdinalIgnoreCase),
        SpawnArea = obj.SpawnArea!,
        BlackList = obj.BlackList.ToListCast<IPoint>(),
        Direction = obj.Direction,
        SpawnTimer = obj.IntervalVariancePct.HasValue
            ? new RandomizedIntervalTimer(TimeSpan.FromSeconds(obj.IntervalSecs), obj.IntervalVariancePct.Value, startAsElapsed: false)
            : new IntervalTimer(TimeSpan.FromSeconds(obj.IntervalSecs), false)
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
            AggroRange = obj.AggroRange,
            AssailIntervalMs = obj.AssailIntervalMs,
            SkillIntervalMs = obj.SkillIntervalMs,
            SpellIntervalMs = obj.SpellIntervalMs,
            MoveIntervalMs = obj.MoveIntervalMs,
            Name = obj.Name,
            ExpReward = obj.ExpReward,
            MaxGoldDrop = obj.MaxGoldDrop,
            MinGoldDrop = obj.MinGoldDrop,
            ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
            SkillTemplateKeys = new HashSet<string>(obj.SkillTemplateKeys, StringComparer.OrdinalIgnoreCase),
            SpellTemplateKeys = new HashSet<string>(obj.SpellTemplateKeys, StringComparer.OrdinalIgnoreCase),
            Sprite = obj.Sprite,
            StatSheet = Mapper.Map<StatSheet>(obj.StatSheet),
            TemplateKey = obj.TemplateKey,
            Type = obj.Type,
            WanderIntervalMs = obj.WanderIntervalMs,
            ScriptVars = new Dictionary<string, IScriptVars>(
                obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
                StringComparer.OrdinalIgnoreCase)
        };
    }

    /// <inheritdoc />
    public MonsterTemplateSchema Map(MonsterTemplate obj) => throw new NotImplementedException();
}