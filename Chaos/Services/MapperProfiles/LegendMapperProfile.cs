using Chaos.Models.Legend;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Time;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class LegendMapperProfile : IMapperProfile<LegendMark, LegendMarkSchema>,
                                          IMapperProfile<LegendMark, LegendMarkInfo>
{
    public LegendMark Map(LegendMarkSchema obj) => new(
        obj.Text,
        obj.Key,
        obj.Icon,
        obj.Color,
        obj.Count,
        GameTime.FromDateTime(obj.Added));

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
        Added = obj.Added.ToDateTime(),
        Color = obj.Color,
        Count = obj.Count,
        Icon = obj.Icon,
        Key = obj.Key,
        Text = obj.Text
    };
}