using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class SpellTemplateTypeMapper : ITypeMapper<SpellTemplate, SpellTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public SpellTemplateTypeMapper(ITypeMapper mapper) => Mapper = mapper;

    public SpellTemplate Map(SpellTemplateSchema obj) => new(obj, Mapper);
    public SpellTemplateSchema Map(SpellTemplate obj) => throw new NotImplementedException();
}