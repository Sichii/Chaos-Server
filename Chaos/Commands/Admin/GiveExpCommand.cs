using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Commands.Admin;

[Command("giveExp")]
public class GiveExpCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IExperienceDistributionScript ExperienceDistributionScript;

    public GiveExpCommand(IClientRegistry<IWorldClient> clientRegistry)
    {
        ClientRegistry = clientRegistry;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<int>(out var amount))
        {
            source.SendOrangeBarMessage($"You gave yourself {amount} exp");
            ExperienceDistributionScript.GiveExp(source, amount);

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
            ExperienceDistributionScript.GiveExp(target, amount);
        }

        return default;
    }
}