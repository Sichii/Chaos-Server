using Chaos.Collections;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Services.Factories;

public sealed class GuildFactory : IGuildFactory
{
    private readonly IChannelService ChannelService;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public GuildFactory(IChannelService channelService, IClientRegistry<IWorldClient> clientRegistry)
    {
        ChannelService = channelService;
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public Guild Create(string name) => new(name, ChannelService, ClientRegistry);
}