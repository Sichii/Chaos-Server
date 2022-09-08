using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class SkillTemplateMapperProfile : IMapperProfile<SkillTemplate, SkillTemplateSchema>
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
        Animation = obj.Animation == null ? null : Mapper.Map<Animation>(obj.Animation),
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value)
    };

    public SkillTemplateSchema Map(SkillTemplate obj) => throw new NotImplementedException();
}