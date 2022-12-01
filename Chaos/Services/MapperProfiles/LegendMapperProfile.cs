using Chaos.Containers;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Legend;
using Chaos.Schemas.Aisling;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class LegendMapperProfile : IMapperProfile<Legend, LegendSchema>,
                                          IMapperProfile<LegendMark, LegendMarkSchema>,
                                          IMapperProfile<LegendMark, LegendMarkInfo>
{
    private readonly ITypeMapper Mapper;
    public LegendMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public Legend Map(LegendSchema obj) => new(Mapper.MapMany<LegendMark>(obj));

    public LegendSchema Map(Legend obj) => new(Mapper.MapMany<LegendMarkSchema>(obj));

    public LegendMark Map(LegendMarkSchema obj) => new(
        obj.Text,
        obj.Key,
        obj.Icon,
        obj.Color,
        obj.Count,
        new GameTime(obj.Added));

    public LegendMark Map(LegendMarkInfo obj) => throw new NotImplementedException();

    LegendMarkInfo IMapperProfile<LegendMark, LegendMarkInfo>.Map(LegendMark obj) => new()
    {
        Color = obj.Color,
        Icon = obj.Icon,
        Key = obj.Key,
        Text = obj.ToString()
    };

    public LegendMarkSchema Map(LegendMark obj) => new()
    {
        Added = obj.Added.Ticks,
        Color = obj.Color,
        Count = obj.Count,
        Icon = obj.Icon,
        Key = obj.Key,
        Text = obj.Text
    };
}