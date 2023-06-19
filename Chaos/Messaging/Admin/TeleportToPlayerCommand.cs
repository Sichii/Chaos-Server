using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("tpto", helpText: "<targetName>")]
public class TeleportToPlayerCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    public TeleportToPlayerCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var playerName))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var player = ClientRegistry
                     .Select(c => c.Aisling)
                     .FirstOrDefault(a => a.Name.EqualsI(playerName));

        if (player == null)
        {
            source.SendOrangeBarMessage($"{playerName} is not online");

            return default;
        }

        source.TraverseMap(player.MapInstance, player, true);

        return default;
    }
}