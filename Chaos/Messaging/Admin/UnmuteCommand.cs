#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("unMute", helpText: "<channelName>")]
public class UnmuteCommand : ICommand<Aisling>
{
    private readonly IChannelService ChannelService;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

    public UnmuteCommand(IChannelService channelService, IClientRegistry<IChaosWorldClient> clientRegistry)
    {
        ChannelService = channelService;
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        type = type.ToLower();

        switch (type)
        {
            case "player":
                if (!args.TryGetNext<string>(out var playerName))
                    return default;

                var player = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(playerName));

                if (player is null)
                    return default;

                player.Aisling.Muted = false;

                break;
            case "channel":
                if (!args.TryGetNext<string>(out var channelName))
                    return default;

                ChannelService.UnmuteChannel(channelName);

                break;
        }

        return default;
    }
}