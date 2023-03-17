using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;

[Command("giveGold")]
public class GiveGoldCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public GiveGoldCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<int>(out var amount))
        {
            if (source.TryGiveGold(amount))
                source.SendOrangeBarMessage($"You gave yourself {amount} gold");

            return default;
        }

        if (args.TryGetNext<string>(out var targetName) && args.TryGetNext(out amount))
        {
            var target = ClientRegistry.Select(client => client.Aisling).FirstOrDefault(aisling => aisling.Name.EqualsI(targetName));

            if (target == null)
            {
                source.SendOrangeBarMessage($"{targetName} is not online");

                return default;
            }

            if (target.TryGiveGold(amount))
                source.SendOrangeBarMessage($"You gave {target.Name} {amount} gold");
        }

        return default;
    }
}