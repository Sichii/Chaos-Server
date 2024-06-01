using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("summon", helpText: "<targetName>")]
public class SummonCommand(IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var playerName))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var aisling = ClientRegistry.Select(c => c.Aisling)
                                    .FirstOrDefault(a => a.Name.EqualsI(playerName));

        if (aisling == null)
            source.SendOrangeBarMessage($"{aisling} is not online");
        else
            aisling.TraverseMap(source.MapInstance, source, true);

        return default;
    }
}