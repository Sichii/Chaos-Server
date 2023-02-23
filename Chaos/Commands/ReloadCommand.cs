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
                        try
                        {
                            await ServiceProvider.ReloadSkillsAsync();
                            aisling.SendOrangeBarMessage("Skills reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload skills");
                            Logger.LogError(e, "Failed to reload skills");
                        }
                    });

                break;
            case "spells":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadSpellsAsync();
                            aisling.SendOrangeBarMessage("Spells reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload spells");
                            Logger.LogError(e, "Failed to reload spells");
                        }
                    });

                break;
            case "items":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadItemsAsync();
                            aisling.SendOrangeBarMessage("Items reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload items");
                            Logger.LogError(e, "Failed to reload items");
                        }
                    });

                break;
            case "monsters":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMonstersAsync();
                            aisling.SendOrangeBarMessage("Monsters reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload monsters");
                            Logger.LogError(e, "Failed to reload monsters");
                        }
                    });

                break;
            case "merchants":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMerchantsAsync();
                            aisling.SendOrangeBarMessage("Merchants reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload merchants");
                            Logger.LogError(e, "Failed to reload merchants");
                        }
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
                        try
                        {
                            await ServiceProvider.ReloadDialogsAsync();
                            aisling.SendOrangeBarMessage("Dialogs reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload dialogs");
                            Logger.LogError(e, "Failed to reload dialogs");
                        }
                    });

                break;
            case "worldmaps":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadWorldMapsAsync();
                            aisling.SendOrangeBarMessage("WorldMaps reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload worldmaps");
                            Logger.LogError(e, "Failed to reload worldmaps");
                        }
                    });

                break;
            case "loottables":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadLootTablesAsync();
                            aisling.SendOrangeBarMessage("LootTables reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload loottables");
                            Logger.LogError(e, "Failed to reload loottables");
                        }
                    });

                break;
        }

        return default;
    }
}