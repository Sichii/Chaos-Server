using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class SpellBookMapperProfile : IMapperProfile<SpellBook, SpellBookSchema>
{
    private readonly ITypeMapper Mapper;
    public SpellBookMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SpellBook Map(SpellBookSchema obj) => new(Mapper.MapMany<Spell>(obj));

    public SpellBookSchema Map(SpellBook obj) => new(Mapper.MapMany<SpellSchema>(obj));
}