using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class MonsterTemplateMapperProfile : IMapperProfile<MonsterTemplate, MonsterTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public MonsterTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    /// <inheritdoc />
    public MonsterTemplate Map(MonsterTemplateSchema obj) => new()
    {
        AttackIntervalMs = obj.AttackIntervalMs,
        CastIntervalMs = obj.CastIntervalMs,
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
        WanderingIntervalMs = obj.WanderIntervalMs
    };

    /// <inheritdoc />
    public MonsterTemplateSchema Map(MonsterTemplate obj) => throw new NotImplementedException();
}