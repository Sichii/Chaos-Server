#region
using Chaos.Collections;
using Chaos.Messaging.Abstractions;
using Chaos.Networking;
using Chaos.Networking.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockGuild
{
    public static Guild Create(
        string name = "TestGuild",
        IChannelService? channelService = null,
        IClientRegistry<IChaosWorldClient>? clientRegistry = null,
        Action<Guild>? setup = null)
    {
        channelService ??= MockChannelService.Create();
        clientRegistry ??= new ClientRegistry<IChaosWorldClient>();

        var guild = new Guild(
            name,
            Guid.NewGuid()
                .ToString("N"),
            channelService,
            clientRegistry);

        setup?.Invoke(guild);

        return guild;
    }
}