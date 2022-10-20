using Chaos.Common.Collections;
using Chaos.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class SkillTemplateMapperProfile : IMapperProfile<SkillTemplate, SkillTemplateSchema>
{
    private readonly ITypeMapper Mapper;

    public SkillTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SkillTemplate Map(SkillTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        IsAssail = obj.IsAssail,
        PanelSprite = obj.PanelSprite,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        ScriptVars = new Dictionary<string, DynamicVars>(
            obj.ScriptVars ?? Enumerable.Empty<KeyValuePair<string, DynamicVars>>(),
            StringComparer.OrdinalIgnoreCase)
    };

    public SkillTemplateSchema Map(SkillTemplate obj) => throw new NotImplementedException();
}