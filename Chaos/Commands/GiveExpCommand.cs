using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.RuntimeScripts;

namespace Chaos.Commands;

[Command("giveExp")]
public class GiveExpCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public GiveExpCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<int>(out var amount))
        {
            source.SendOrangeBarMessage($"You gave yourself {amount} exp");
            ExperienceDistributionScripts.Default.GiveExp(source, amount);

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

            source.SendOrangeBarMessage($"You gave {target.Name} {amount} exp");
            ExperienceDistributionScripts.Default.GiveExp(target, amount);
        }

        return default;
    }
}