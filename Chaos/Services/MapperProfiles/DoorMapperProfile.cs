using Chaos.Models.World;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class DoorMapperProfile : IMapperProfile<Door, DoorInfo>
{
    public Door Map(DoorInfo obj) => throw new NotImplementedException();

    public DoorInfo Map(Door obj)
        => new()
        {
            X = obj.X,
            Y = obj.Y,
            Closed = obj.Closed,
            OpenRight = obj.OpenRight
        };
}