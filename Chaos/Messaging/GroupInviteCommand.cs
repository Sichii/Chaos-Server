using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Other.Abstractions;

namespace Chaos.Messaging;

[Command("invite", false, "<targetName>")]
public class GroupInviteCommand(IClientRegistry<IWorldClient> clientRegistry, IGroupService groupService) : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry = clientRegistry;
    private readonly IGroupService GroupService = groupService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var targetName))
            return default;

        var targetClient = ClientRegistry.FirstOrDefault(c => c.Aisling.Name.EqualsI(targetName));

        if ((targetClient == null) || targetClient.Aisling.IsAdmin)
        {
            source.SendOrangeBarMessage($"{targetName} can not be found");

            return default;
        }

        GroupService.Invite(source, targetClient.Aisling);

        return default;
    }
}