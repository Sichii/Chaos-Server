using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Other.Abstractions;

namespace Chaos.Commands;

[Command("invite", false)]
public class GroupInviteCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IGroupService GroupService;

    public GroupInviteCommand(IClientRegistry<IWorldClient> clientRegistry, IGroupService groupService)
    {
        ClientRegistry = clientRegistry;
        GroupService = groupService;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return default;

        var targetClient = ClientRegistry.FirstOrDefault(c => c.Aisling.Name.EqualsI(name));

        if ((targetClient == null) || targetClient.Aisling.IsAdmin)
        {
            source.SendOrangeBarMessage($"{name} can not be found");

            return default;
        }

        GroupService.Invite(source, targetClient.Aisling);

        return default;
    }
}