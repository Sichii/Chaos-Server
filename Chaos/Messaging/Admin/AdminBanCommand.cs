using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Security.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("adminban", helpText: "<name>")]
public class AdminBanCommand(IClientRegistry<IWorldClient> clientRegistry, IAccessManager accessManager) : ICommand<Aisling>
{
    private readonly IAccessManager AccessManager = accessManager;
    private readonly IClientRegistry<IWorldClient> ClientRegistry = clientRegistry;

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