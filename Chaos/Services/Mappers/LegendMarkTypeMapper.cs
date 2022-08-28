using Chaos.Data;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class LegendMarkTypeMapper : ITypeMapper<LegendMark, LegendMarkSchema>
{
    public LegendMark Map(LegendMarkSchema obj) => new(obj);

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