using Chaos.Data;
using Chaos.Entities.Schemas.Data;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class WarpMapperProfile : IMapperProfile<Warp, WarpSchema>
{
    /// <inheritdoc />
    public Warp Map(WarpSchema obj) => new()
    {
        SourceLocation = new Location(null!, obj.Source.X, obj.Source.Y),
        TargetLocation = obj.Destination
    };

    /// <inheritdoc />
    public WarpSchema Map(Warp obj) => throw new NotImplementedException();
}