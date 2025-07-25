#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Security.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("adminidban", helpText: "<name>")]
public class AdminIdBanCommand(IClientRegistry<IChaosWorldClient> clientRegistry, IAccessManager accessManager) : ICommand<Aisling>
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

        //don't banish the default values
        if (targetClient is { LoginId1: 4278255360U, LoginId2: 7695 })
        {
            source.SendOrangeBarMessage($"{targetClient.Aisling.Name} can not be ID banned due to having default ids");

            await IpBan(targetClient);

            source.SendOrangeBarMessage($"{targetClient.Aisling.Name} has been IP banned instead");

            return;
        }

        await AccessManager.IdBanishAsync(targetClient.LoginId1, targetClient.LoginId2);
        targetClient.Disconnect();

        source.SendOrangeBarMessage($"{targetClient.Aisling.Name} has been ID banned");
    }

    private async Task IpBan(IChaosWorldClient targetClient)
    {
        await AccessManager.IpBanishAsync(targetClient.RemoteIp);
        targetClient.Disconnect();
    }
}