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
        if (!args.TryGetNext<string>(out var subCommand))
            return default;

        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (subCommand.ToLower())
        {
            case "skills":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadSkillsAsync();

                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Skills reloaded");
                    });

                break;
            case "spells":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadSpellsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Spells reloaded");
                    });

                break;
            case "items":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadItemsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Items reloaded");
                    });

                break;
            case "monsters":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMonstersAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Monsters reloaded");
                    });

                break;
            case "merchants":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMerchantsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Merchants reloaded");
                    });

                break;
            case "maps":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMapsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Maps reloaded");
                    });

                break;
            case "dialogs":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadDialogsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Dialogs reloaded");
                    });

                break;
            case "worldmaps":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadWorldMapsAsync();
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "WorldMaps reloaded");
                    });

                break;
        }

        return default;
    }
}