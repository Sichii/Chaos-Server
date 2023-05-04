using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Security.Abstractions;

namespace Chaos.Commands.Admin;

[Command("adminban")]
public class AdminBanCommand : ICommand<Aisling>
{
    private readonly IAccessManager AccessManager;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public AdminBanCommand(IClientRegistry<IWorldClient> clientRegistry, IAccessManager accessManager)
    {
        ClientRegistry = clientRegistry;
        AccessManager = accessManager;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return;

        var targetClient = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));

        if ((targetClient == null) || targetClient.Aisling.IsAdmin)
        {
            source.SendOrangeBarMessage($"{name} can not be found");

            return;
        }

        await AccessManager.BanishAsync(targetClient.RemoteIp);
        targetClient.Disconnect();
    }
}