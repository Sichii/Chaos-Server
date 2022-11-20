using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;

namespace Chaos.Commands;

[Command("tpto")]
public sealed class TeleportCommand : ICommand<Aisling>
{
    private readonly IServiceProvider Provider;
    private readonly ISimpleCache Cache;
    public TeleportCommand(ISimpleCache cache, IServiceProvider provider)
    {
        Cache = cache;
        Provider = provider;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(Aisling aisling, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return;

        switch (type.ToLower())
        {
            case "player":
                if (!args.TryGetNext<string>(out var playerName))
                    return;

                var player = await Provider.GetAislingsAsync().FirstOrDefaultAsync(a => a.Name.EqualsI(playerName));

                if (player == null)
                {
                    aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{playerName} is not online");

                    return;
                }
                
                aisling.TraverseMap(player.MapInstance, player);

                break;
            case "map":
                if(!args.TryGetNext<string>(out var mapInstanceId))
                    return;

                var mapInstance = Cache.Get<MapInstance>(mapInstanceId);
                
                Point point;

                if (args.TryGetNext<int>(out var xPos) && args.TryGetNext<int>(out var yPos))
                    point = new Point(xPos, yPos);
                else
                    point = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);

                aisling.TraverseMap(mapInstance, point);

                return;
        }
    }
}