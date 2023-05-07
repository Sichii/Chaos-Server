using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;

namespace Chaos.Extensions;

public static class WorldClientExtensions
{
    public static void SendDoors(this IWorldClient worldClient, params Door[] doors) => worldClient.SendDoors(doors);

    public static void SendVisibleEntities(this IWorldClient worldClient, params VisibleEntity[] objects) =>
        worldClient.SendVisibleEntities(objects);
}