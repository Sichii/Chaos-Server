using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class MonsterTemplateMapperProfile : IMapperProfile<MonsterTemplate, MonsterTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public MonsterTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

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
            WanderIntervalMs = obj.WanderIntervalMs
        };
    }

    /// <inheritdoc />
    public MonsterTemplateSchema Map(MonsterTemplate obj) => throw new NotImplementedException();
}