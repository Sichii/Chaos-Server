using Chaos.Clients.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Extensions;

public static class WorldClientExtensions
{
    public static void SendDoors(this IWorldClient worldClient, params Door[] doors) => worldClient.SendDoors(doors);

    public static void SendVisibleObjects(this IWorldClient worldClient, params VisibleEntity[] objects) =>
        worldClient.SendVisibleEntities(objects);
}