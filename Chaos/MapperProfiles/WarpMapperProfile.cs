using Chaos.Data;
using Chaos.Entities.Schemas.Data;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class WarpMapperProfile : IMapperProfile<Warp, WarpSchema>
{
    /// <inheritdoc />
    public Warp Map(WarpSchema obj) => new()
    {
        Source = new Point(obj.Source.X, obj.Source.Y),
        Destination = obj.Destination
    };

    /// <inheritdoc />
    public WarpSchema Map(Warp obj) => throw new NotImplementedException();
}