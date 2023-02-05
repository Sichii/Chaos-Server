using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("revive")]
public class ReviveCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public ReviveCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return;

        var client = ClientRegistry.FirstOrDefault(cli => cli.Aisling?.Name.EqualsI(name) ?? false);

        if (client == null)
            return;

        var aisling = client.Aisling;

        await using var @lock = await aisling.MapInstance.Sync.WaitAsync();

        aisling.IsDead = false;
        aisling.StatSheet.SetHealthPct(100);
        aisling.StatSheet.SetManaPct(100);
        aisling.Refresh(true);
    }
}