using Chaos.Data;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class LegendMarkMapperProfile : IMapperProfile<LegendMark, LegendMarkSchema>,
                                              IMapperProfile<LegendMark, LegendMarkInfo>
{
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