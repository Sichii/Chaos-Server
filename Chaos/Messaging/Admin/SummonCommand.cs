#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Other.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("summon", helpText: "<targetName>")]
public class SummonCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IMapTraversalService MapTraversalService;

    public SummonCommand(IClientRegistry<IChaosWorldClient> clientRegistry, IMapTraversalService mapTraversalService)
    {
        ClientRegistry = clientRegistry;
        MapTraversalService = mapTraversalService;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var playerName))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var aisling = ClientRegistry.Select(c => c.Aisling)
                                    .FirstOrDefault(a => a.Name.EqualsI(playerName));

        if (aisling == null)
            source.SendOrangeBarMessage($"{playerName} is not online");
        else
            MapTraversalService.AdminTraverseMap(aisling, source.MapInstance, source);

        return default;
    }
}