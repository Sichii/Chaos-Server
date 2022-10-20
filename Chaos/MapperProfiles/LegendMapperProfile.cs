using Chaos.Containers;
using Chaos.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class LegendMapperProfile : IMapperProfile<Legend, LegendSchema>
{
    private readonly ITypeMapper Mapper;
    public LegendMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public Legend Map(LegendSchema obj) => new(Mapper.MapMany<LegendMark>(obj));

    public LegendSchema Map(Legend obj) => new(Mapper.MapMany<LegendMarkSchema>(obj));
}