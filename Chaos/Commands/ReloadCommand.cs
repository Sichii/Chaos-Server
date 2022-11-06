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
    public ValueTask ExecuteAsync(Aisling aisling, ArgumentCollection args)
    {
        var arg = args.FirstOrDefault();

        if (arg == null)
            return default;

        switch (arg.ToLower())
        {
            case "skills":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadSkills();

                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Skills reloaded");
                    });

                break;
            case "spells":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadSpells();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Spells reloaded");
                    });

                break;
            case "items":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadItems();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Items reloaded");
                    });

                break;
            case "monsters":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMonsters();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Monsters reloaded");
                    });

                break;
            case "merchants":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMerchants();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Merchants reloaded");
                    });

                break;
            case "maps":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMaps();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Maps reloaded");
                    });

                break;
            case "dialogs":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadDialogs();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Dialogs reloaded");
                    });

                break;
        }

        return default;
    }
}