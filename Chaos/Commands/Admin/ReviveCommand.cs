using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;

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
        IPolyDisposable? @lock = null;

        if (!source.MapInstance.Equals(aisling.MapInstance))
            @lock = await aisling.MapInstance.Sync.WaitAsync();

        try
        {
            aisling.IsDead = false;
            aisling.StatSheet.SetHealthPct(100);
            aisling.StatSheet.SetManaPct(100);
            aisling.Refresh(true);
        } finally
        {
            @lock?.Dispose();
        }
    }
}