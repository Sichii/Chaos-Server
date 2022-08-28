using Chaos.Entities.Networking.Server;
using Chaos.Objects.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class DoorTypeMapper : ITypeMapper<Door, DoorInfo>
{
    public Door Map(DoorInfo obj) => throw new NotImplementedException();

    public DoorInfo Map(Door obj) => new()
    {
        X = obj.X,
        Y = obj.Y,
        Closed = obj.Closed,
        OpenRight = obj.OpenRight
    };
}