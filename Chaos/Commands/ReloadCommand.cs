using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Extensions;
using Chaos.Objects.World;
using Microsoft.Extensions.Logging;

namespace Chaos.Commands;

[Command("reload")]
public sealed class ReloadCommand : ICommand<Aisling>
{
    private readonly ILogger<ReloadCommand> Logger;
    private readonly IServiceProvider ServiceProvider;

    public ReloadCommand(IServiceProvider serviceProvider, ILogger<ReloadCommand> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

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

                        aisling.SendOrangeBarMessage("Skills reloaded");
                    });

                break;
            case "spells":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadSpellsAsync();
                        aisling.SendOrangeBarMessage("Spells reloaded");
                    });

                break;
            case "items":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadItemsAsync();
                        aisling.SendOrangeBarMessage("Items reloaded");
                    });

                break;
            case "monsters":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMonstersAsync();
                        aisling.SendOrangeBarMessage("Monsters reloaded");
                    });

                break;
            case "merchants":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadMerchantsAsync();
                        aisling.SendOrangeBarMessage("Merchants reloaded");
                    });

                break;
            case "maps":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMapsAsync();
                            aisling.SendOrangeBarMessage("Maps reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload maps");
                            Logger.LogError(e, "Failed to reload maps");
                        }
                    });

                break;
            case "dialogs":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadDialogsAsync();
                        aisling.SendOrangeBarMessage("Dialogs reloaded");
                    });

                break;
            case "worldmaps":
                _ = Task.Run(
                    async () =>
                    {
                        await ServiceProvider.ReloadWorldMapsAsync();
                        aisling.SendOrangeBarMessage("WorldMaps reloaded");
                    });

                break;
        }

        return default;
    }
}