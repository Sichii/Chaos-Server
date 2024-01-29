using Chaos.Collections.Common;
using Chaos.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;

namespace Chaos.Messaging.Admin;

[Command("reload", helpText: "<skills|spells|items|monsters|merchants|maps|dialogs|worldMaps|loottables>")]
public sealed class ReloadCommand(IServiceProvider serviceProvider, ILogger<ReloadCommand> logger) : ICommand<Aisling>
{
    private readonly ILogger<ReloadCommand> Logger = logger;
    private readonly IServiceProvider ServiceProvider = serviceProvider;

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
                            await ServiceProvider.ReloadSkillsAsync(Logger);
                            aisling.SendOrangeBarMessage("Skills reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload skills");

                            Logger.WithTopics(Topics.Entities.Skill, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload skills");
                        }
                    });

                break;
            case "spells":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadSpellsAsync(Logger);
                            aisling.SendOrangeBarMessage("Spells reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload spells");

                            Logger.WithTopics(Topics.Entities.Spell, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload spells");
                        }
                    });

                break;
            case "items":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadItemsAsync(Logger);
                            aisling.SendOrangeBarMessage("Items reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload items");

                            Logger.WithTopics(Topics.Entities.Item, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload items");
                        }
                    });

                break;
            case "monsters":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMonstersAsync(Logger);
                            aisling.SendOrangeBarMessage("Monsters reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload monsters");

                            Logger.WithTopics(Topics.Entities.Monster, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload monsters");
                        }
                    });

                break;
            case "merchants":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMerchantsAsync(Logger);
                            aisling.SendOrangeBarMessage("Merchants reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload merchants");

                            Logger.WithTopics(Topics.Entities.Merchant, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload merchants");
                        }
                    });

                break;
            case "maps":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadMapsAsync(Logger);
                            aisling.SendOrangeBarMessage("Maps reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload maps");

                            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.MapTemplate, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload maps");
                        }
                    });

                break;
            case "dialogs":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadDialogsAsync(Logger);
                            aisling.SendOrangeBarMessage("Dialogs reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload dialogs");

                            Logger.WithTopics(Topics.Entities.Dialog, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload dialogs");
                        }
                    });

                break;
            case "worldmaps":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadWorldMapsAsync(Logger);
                            aisling.SendOrangeBarMessage("WorldMaps reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload worldmaps");

                            Logger.WithTopics(Topics.Entities.WorldMap, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload worldmaps");
                        }
                    });

                break;
            case "loottables":
                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await ServiceProvider.ReloadLootTablesAsync(Logger);
                            aisling.SendOrangeBarMessage("LootTables reloaded");
                        } catch (Exception e)
                        {
                            aisling.SendOrangeBarMessage("Failed to reload loottables");

                            Logger.WithTopics(Topics.Entities.LootTable, Topics.Actions.Reload)
                                  .LogError(e, "Failed to reload loottables");
                        }
                    });

                break;
        }

        return default;
    }
}