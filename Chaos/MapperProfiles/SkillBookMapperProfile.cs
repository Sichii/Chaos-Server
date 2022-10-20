using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class SkillBookMapperProfile : IMapperProfile<SkillBook, SkillBookSchema>
{
    private readonly ITypeMapper Mapper;
    public SkillBookMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public SkillBook Map(SkillBookSchema obj) => new(Mapper.MapMany<Skill>(obj));

    public SkillBookSchema Map(SkillBook obj) => new(Mapper.MapMany<SkillSchema>(obj));
}