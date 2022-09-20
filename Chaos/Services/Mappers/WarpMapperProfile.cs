using Chaos.Data;
using Chaos.Entities.Schemas.Data;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

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