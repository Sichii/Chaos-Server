#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Other.Abstractions;
#endregion

namespace Chaos.Messaging;

[Command("kick", false, "<targetName>")]
public class KickCommand(IGroupService groupService, IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;
    private readonly IGroupService GroupService = groupService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var targetName))
            return default;

        targetName = targetName.ReplaceI("_", " ");

        var targetClient = ClientRegistry.FirstOrDefault(c => c.Aisling.Name.EqualsI(targetName));

        if ((targetClient == null) || (source.IsAdmin != targetClient.Aisling.IsAdmin))
        {
            source.SendOrangeBarMessage($"{targetName} can not be found");

            return default;
        }

        GroupService.Kick(source, targetClient.Aisling);

        return default;
    }
}