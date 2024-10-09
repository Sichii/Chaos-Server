using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Utilities;

namespace Chaos.Messaging.Admin;

[Command("chaos", helpText: "<message> <mins>")]
public class ChaosCommand(IServiceProvider serviceProvider, IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;
    private readonly IServiceProvider ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var initialMessage))
            return default;

        if (!args.TryGetNext<int>(out var mins))
            return default;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(ServerMessageType.AdminMessage, initialMessage);

        ShutdownUtility.BeginShutdown(ServiceProvider, mins);

        return default;
    }
}