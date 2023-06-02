using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Models.WorldMap;
using Chaos.Schemas.Aisling;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task ReloadDialogsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var dialogCache = cacheProvider.GetCache<DialogTemplate>();

        await dialogCache.ReloadAsync();
    }

    public static async Task ReloadItemsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var itemTemplateCache = cacheProvider.GetCache<ItemTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        await itemTemplateCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            foreach (var groundItem in mapInstance.GetEntities<GroundItem>().ToList())
            {
                var schema = mapper.Map<ItemSchema>(groundItem.Item);
                var item = mapper.Map<Item>(schema);

                groundItem.Item = item;
            }

            foreach (var creature in mapInstance.GetEntities<Creature>())
                switch (creature)
                {
                    case Aisling aisling:
                    {
                        //if the aisling has an exchange open, cancel it
                        var exchange = aisling.ActiveObject.TryGet<Exchange>();
                        exchange?.Cancel(aisling);

                        var inventorySchemas = mapper.MapMany<ItemSchema>(aisling.Inventory);
                        var inventoryItems = mapper.MapMany<Item>(inventorySchemas).ToList();

                        foreach (var item in inventoryItems)
                        {
                            aisling.Inventory.Remove(item.Slot);
                            aisling.Inventory.TryAddDirect(item.Slot, item);
                        }

                        var equipmentSchemas = mapper.MapMany<ItemSchema>(aisling.Equipment);
                        var equipmentItems = mapper.MapMany<Item>(equipmentSchemas);

                        foreach (var item in equipmentItems)
                        {
                            aisling.Equipment.Remove(item.Slot);
                            aisling.Equipment.TryAdd(item.Slot, item);
                        }

                        break;
                    }
                    case Monster monster:
                    {
                        {
                            var schemas = mapper.MapMany<ItemSchema>(monster.Items);
                            var items = mapper.MapMany<Item>(schemas).ToList();

                            monster.Items.Clear();
                            monster.Items.AddRange(items);
                        }

                        break;
                    }
                }
        }
    }

    public static async Task ReloadLootTablesAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var lootTableCache = cacheProvider.GetCache<LootTable>();
        var mapCache = cacheProvider.GetCache<MapInstance>();

        await lootTableCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            foreach (var monsterSpawn in mapInstance.MonsterSpawns)
            {
                if (monsterSpawn.LootTable?.Key == null)
                    continue;

                var lootTableKey = monsterSpawn.LootTable.Key;
                var newLootTable = lootTableCache.Get(lootTableKey);
                monsterSpawn.LootTable = newLootTable;
            }

            foreach (var monster in mapInstance.GetEntities<Monster>())
            {
                if (monster.LootTable == null)
                    continue;

                var lootTableKey = monster.LootTable.Key;
                var newLootTable = lootTableCache.Get(lootTableKey);
                monster.LootTable = newLootTable;
            }
        }
    }

    public static async Task ReloadMapsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var oldMaps = mapCache.ToDictionary(m => m.InstanceId, StringComparer.OrdinalIgnoreCase);

        //locks ALL maps
        await using var oldSync = await ComplexSynchronizationHelper.WaitAsync(
            TimeSpan.FromSeconds(1),
            TimeSpan.FromMilliseconds(5),
            oldMaps.Values.Select(m => m.Sync).ToArray());

        await mapCache.ReloadAsync();

        var newMaps = mapCache.ToDictionary(m => m.InstanceId, StringComparer.OrdinalIgnoreCase);

        await using var newSync = await ComplexSynchronizationHelper.WaitAsync(
            TimeSpan.FromSeconds(5),
            TimeSpan.FromMilliseconds(50),
            newMaps.Values.Select(m => m.Sync).ToArray());

        foreach (var oldMap in oldMaps.Values)
            try
            {
                var newMap = mapCache.Get(oldMap.InstanceId);

                foreach (var monster in newMap.GetEntities<Monster>())
                    newMap.RemoveObject(monster);

                foreach (var groundEntity in oldMap.GetEntities<GroundEntity>())
                    newMap.SimpleAdd(groundEntity);

                foreach (var monster in oldMap.GetEntities<Monster>())
                    newMap.SimpleAdd(monster);

                foreach (var aisling in oldMap.GetEntities<Aisling>())
                    newMap.SimpleAdd(aisling);

                oldMap.Destroy();

                newMap.BaseInstanceId = oldMap.BaseInstanceId;
            } catch (Exception e)
            {
                logger.WithProperty(oldMap)
                      .LogError(e, "Failed to migrate map {@MapInstanceId} during reload", oldMap.InstanceId);
            }
    }

    public static async Task ReloadMerchantsAsync(this IServiceProvider provider, ILogger logger)
    {
        var merchantFactory = provider.GetRequiredService<IMerchantFactory>();
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var merchantTemplateCache = cacheProvider.GetCache<MerchantTemplate>();

        await merchantTemplateCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            var merchantsToAdd = new List<Merchant>();

            foreach (var merchant in mapInstance.GetEntities<Merchant>().ToList())
                try
                {
                    var extraScriptKeys = merchant.ScriptKeys.Except(merchant.Template.ScriptKeys);

                    var newMerchant = merchantFactory.Create(
                        merchant.Template.TemplateKey,
                        merchant.MapInstance,
                        merchant,
                        merchant.Template.ScriptKeys.Concat(extraScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase));

                    newMerchant.Direction = merchant.Direction;

                    merchant.MapInstance.RemoveObject(merchant);
                    merchantsToAdd.Add(newMerchant);
                } catch (Exception e)
                {
                    logger.WithProperty(merchant)
                          .WithProperty(mapInstance)
                          .LogError(
                              e,
                              "Failed to migrate merchant {@MerchantTemplateKey} on map {@MapInstanceId} during reload",
                              merchant.Template.TemplateKey,
                              mapInstance.InstanceId);
                }

            mapInstance.AddObjects(merchantsToAdd);
        }
    }

    public static async Task ReloadMonstersAsync(this IServiceProvider provider, ILogger logger)
    {
        var monsterFactory = provider.GetRequiredService<IMonsterFactory>();
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var monsterTemplateCache = cacheProvider.GetCache<MonsterTemplate>();

        await monsterTemplateCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            var monstersToAdd = new List<Monster>();

            foreach (var monster in mapInstance.GetEntities<Monster>().ToList())
                try
                {
                    var newMonster = monsterFactory.Create(
                        monster.Template.TemplateKey,
                        monster.MapInstance,
                        monster,
                        monster.ScriptKeys);

                    newMonster.Items.AddRange(monster.Items);
                    newMonster.Gold = monster.Gold;
                    newMonster.Experience = monster.Experience;
                    newMonster.AggroRange = monster.AggroRange;
                    newMonster.LootTable = monster.LootTable;

                    monster.MapInstance.RemoveObject(monster);
                    monstersToAdd.Add(newMonster);
                } catch (Exception e)
                {
                    logger.WithProperty(monster)
                          .WithProperty(mapInstance)
                          .LogError(
                              e,
                              "Failed to migrate monster {@MonsterTemplateKey} on map {@MapInstanceId} during reload",
                              monster.Template.TemplateKey,
                              mapInstance.InstanceId);
                }

            mapInstance.AddObjects(monstersToAdd);
        }
    }

    public static async Task ReloadSkillsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var skillTemplateCache = cacheProvider.GetCache<SkillTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        await skillTemplateCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            foreach (var creature in mapInstance.GetEntities<Creature>())
                switch (creature)
                {
                    case Aisling aisling:
                    {
                        var schemas = mapper.MapMany<SkillSchema>(aisling.SkillBook);
                        var skills = mapper.MapMany<Skill>(schemas).ToList();

                        foreach (var skill in skills)
                        {
                            aisling.SkillBook.Remove(skill.Slot);
                            aisling.SkillBook.TryAdd(skill.Slot, skill);
                        }
                    }

                        break;

                    case Monster monster:
                    {
                        {
                            var schemas = mapper.MapMany<SkillSchema>(monster.Skills);
                            var skills = mapper.MapMany<Skill>(schemas).ToList();

                            monster.Skills.Clear();
                            monster.Skills.AddRange(skills);
                        }
                    }

                        break;
                }
        }
    }

    public static async Task ReloadSpellsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var spellTemplateCache = cacheProvider.GetCache<SpellTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        await spellTemplateCache.ReloadAsync();

        foreach (var mapInstance in mapCache)
        {
            await using var sync = await mapInstance.Sync.WaitAsync();

            foreach (var creature in mapInstance.GetEntities<Creature>())
                switch (creature)
                {
                    case Aisling aisling:
                    {
                        var schemas = mapper.MapMany<SpellSchema>(aisling.SpellBook);
                        var spells = mapper.MapMany<Spell>(schemas).ToList();

                        foreach (var spell in spells)
                        {
                            aisling.SpellBook.Remove(spell.Slot);
                            aisling.SpellBook.TryAdd(spell.Slot, spell);
                        }
                    }

                        break;

                    case Monster monster:
                    {
                        {
                            var schemas = mapper.MapMany<SpellSchema>(monster.Spells);
                            var spells = mapper.MapMany<Spell>(schemas).ToList();

                            monster.Spells.Clear();
                            monster.Spells.AddRange(spells);
                        }
                    }

                        break;
                }
        }
    }

    public static async Task ReloadWorldMapsAsync(this IServiceProvider provider, ILogger logger)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var worldMapNodeCache = cacheProvider.GetCache<WorldMapNode>();
        var worldMapCache = cacheProvider.GetCache<WorldMap>();

        await worldMapNodeCache.ReloadAsync();
        await worldMapCache.ReloadAsync();
    }
}