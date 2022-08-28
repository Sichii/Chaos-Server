using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class SkillTemplateTypeMapper : ITypeMapper<SkillTemplate, SkillTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    
    public SkillTemplateTypeMapper(ITypeMapper mapper) => Mapper = mapper;

    public SkillTemplate Map(SkillTemplateSchema obj) => new(obj, Mapper);
    public SkillTemplateSchema Map(SkillTemplate obj) => throw new NotImplementedException();
}