using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

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
        Animation = obj.Animation == null ? null : Mapper.Map<Animation>(obj.Animation),
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value)
    };
    
    public SpellTemplateSchema Map(SpellTemplate obj) => throw new NotImplementedException();
}