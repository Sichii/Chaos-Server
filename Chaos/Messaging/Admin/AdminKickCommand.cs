using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("adminkick", helpText: "<name>")]
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