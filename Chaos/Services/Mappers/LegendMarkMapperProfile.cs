using Chaos.Data;
using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Time;

namespace Chaos.Services.Mappers;

public class LegendMarkMapperProfile : IMapperProfile<LegendMark, LegendMarkSchema>,
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