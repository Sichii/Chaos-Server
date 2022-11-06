using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Schemas.Aisling;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task ReloadDialogs(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var dialogCache = cacheProvider.GetCache<DialogTemplate>();

        await dialogCache.ReloadAsync();
    }

    public static async Task ReloadItems(this IServiceProvider provider)
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
                            aisling.Inventory.TryAdd(item.Slot, item);
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

    public static async Task ReloadMaps(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var oldMaps = mapCache.ToList();

        await mapCache.ReloadAsync();

        foreach (var oldMap in oldMaps)
        {
            var newMap = mapCache.Get(oldMap.InstanceId);

            await using var newSync = await newMap.Sync.WaitAsync();
            await using var sync = await oldMap.Sync.WaitAsync();

            foreach (var groundEntity in oldMap.GetEntities<GroundEntity>())
                newMap.SimpleAdd(groundEntity);

            foreach (var monster in oldMap.GetEntities<Monster>())
                newMap.SimpleAdd(monster);

            foreach (var aisling in oldMap.GetEntities<Aisling>())
                newMap.AddObject(aisling, aisling);

            oldMap.Destroy();
        }
    }

    public static async Task ReloadMerchants(this IServiceProvider provider)
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
            {
                var newMerchant = merchantFactory.Create(
                    merchant.Template.TemplateKey,
                    merchant.MapInstance,
                    merchant,
                    merchant.ScriptKeys);

                merchant.MapInstance.RemoveObject(merchant);
                merchantsToAdd.Add(newMerchant);
            }

            mapInstance.AddObjects(merchantsToAdd);
        }
    }

    public static async Task ReloadMonsters(this IServiceProvider provider)
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

                monster.MapInstance.RemoveObject(monster);
                monstersToAdd.Add(newMonster);
            }

            mapInstance.AddObjects(monstersToAdd);
        }
    }

    public static async Task ReloadSkills(this IServiceProvider provider)
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

    public static async Task ReloadSpells(this IServiceProvider provider)
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
}