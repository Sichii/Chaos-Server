#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Security.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("adminipban", helpText: "<name>")]
public class AdminIpBanCommand(IClientRegistry<IChaosWorldClient> clientRegistry, IAccessManager accessManager) : ICommand<Aisling>
{
    private readonly IAccessManager AccessManager = accessManager;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;

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

        await AccessManager.IpBanishAsync(targetClient.RemoteIp);
        targetClient.Disconnect();
    }
}