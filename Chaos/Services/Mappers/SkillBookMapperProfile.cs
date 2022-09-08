using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class SkillBookMapperProfile : IMapperProfile<SkillBook, SkillBookSchema>
{
    private readonly ITypeMapper Mapper;
    public SkillBookMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SkillBook Map(SkillBookSchema obj) => new(Mapper.MapMany<Skill>(obj));

    public SkillBookSchema Map(SkillBook obj) => new(Mapper.MapMany<SkillSchema>(obj));
}