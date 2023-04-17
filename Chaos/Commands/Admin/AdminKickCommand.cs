using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;

[Command("adminkick")]
public class AdminKickCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    public AdminKickCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return default;

        var targetClient = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));

        if ((targetClient == null) || targetClient.Aisling.IsAdmin)
        {
            source.SendOrangeBarMessage($"{name} can not be found");

            return default;
        }

        targetClient.Disconnect();

        return default;
    }
}