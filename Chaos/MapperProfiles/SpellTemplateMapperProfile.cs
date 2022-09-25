using Chaos.Common.Collections;
using Chaos.Entities.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class SpellTemplateMapperProfile : IMapperProfile<SpellTemplate, SpellTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public SpellTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SpellTemplate Map(SpellTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        CastLines = obj.CastLines,
        Prompt = obj.Prompt,
        SpellType = obj.SpellType,
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        PanelSprite = obj.PanelSprite,
        ScriptVars = new Dictionary<string, DynamicVars>(
            obj.ScriptVars ?? Enumerable.Empty<KeyValuePair<string, DynamicVars>>(),
            StringComparer.OrdinalIgnoreCase)
    };

    public SpellTemplateSchema Map(SpellTemplate obj) => throw new NotImplementedException();
}