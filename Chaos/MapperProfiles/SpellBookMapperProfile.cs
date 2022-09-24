using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class SpellBookMapperProfile : IMapperProfile<SpellBook, SpellBookSchema>
{
    private readonly ITypeMapper Mapper;
    public SpellBookMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SpellBook Map(SpellBookSchema obj) => new(Mapper.MapMany<Spell>(obj));

    public SpellBookSchema Map(SpellBook obj) => new(Mapper.MapMany<SpellSchema>(obj));
}