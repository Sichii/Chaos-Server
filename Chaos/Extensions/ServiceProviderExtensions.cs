using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Entities.Schemas.Templates;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Servers.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Extensions;

public static class ServiceProviderExtensions
{
    public static void ReloadItems(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var itemTemplateCache = cacheProvider.GetCache<ItemTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        AsyncHelpers.RunSync(() => itemTemplateCache.ReloadAsync());

        foreach (var mapInstance in mapCache)
        {
            using var @lock = mapInstance.Sync.Enter();

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

    public static void ReloadSkills(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var skillTemplateCache = cacheProvider.GetCache<SkillTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        AsyncHelpers.RunSync(() => skillTemplateCache.ReloadAsync());

        foreach (var mapInstance in mapCache)
        {
            using var @lock = mapInstance.Sync.Enter();

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

    public static void ReloadSpells(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var spellTemplateCache = cacheProvider.GetCache<SpellTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        AsyncHelpers.RunSync(() => spellTemplateCache.ReloadAsync());

        foreach (var mapInstance in mapCache)
        {
            using var @lock = mapInstance.Sync.Enter();

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

    public static void ReloadMonsters(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var monsterTemplateCache = cacheProvider.GetCache<MonsterTemplate>();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        AsyncHelpers.RunSync(() => monsterTemplateCache.ReloadAsync());
        
        foreach (var mapInstance in mapCache)
        {
            using var @lock = mapInstance.Sync.Enter();

            var monstersToAdd = new List<Monster>();
            
            foreach (var monster in mapInstance.GetEntities<Monster>().ToList())
            {
                var schema = mapper.Map<MonsterTemplateSchema>(monster);
                var newMonster = mapper.Map<Monster>(schema);

                monster.MapInstance.RemoveObject(monster);
                monstersToAdd.Add(newMonster);
            }

            mapInstance.AddObjects(monstersToAdd);
        }
    }

    public static void ReloadMaps(this IServiceProvider provider)
    {
        var cacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = cacheProvider.GetCache<MapInstance>();
        var loginServer = provider.GetRequiredService<ILoginServer>();
        var worldServer = provider.GetRequiredService<IWorldServer>();
        

        //lock all players
        //reload maps from files
        //copy all entities from old to new
        //unlock all players
        
        
        
        AsyncHelpers.RunSync(() => mapCache.ReloadAsync());
    }
}