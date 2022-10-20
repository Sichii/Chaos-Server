using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("reload")]
public sealed class ReloadCommand : ICommand<Aisling>
{
    private readonly IServiceProvider ServiceProvider;
    public ReloadCommand(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public void Execute(Aisling aisling, ArgumentCollection args)
    {
        var arg = args.FirstOrDefault();

        if (arg == null)
            return;

        switch (arg.ToLower())
        {
            case "skills":
                ServiceProvider.ReloadSkills();
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Skills reloaded");

                break;
            case "spells":
                ServiceProvider.ReloadSpells();
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Spells reloaded");

                break;
            case "items":
                ServiceProvider.ReloadItems();
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Items reloaded");

                break;
            case "monsters":
                ServiceProvider.ReloadMonsters();
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Monsters reloaded");

                break;
            case "maps":
                ServiceProvider.ReloadMaps();
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Maps reloaded");

                break;
        }
    }
}