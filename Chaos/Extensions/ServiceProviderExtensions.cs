using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;
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

            foreach (var creature in mapInstance.GetEntities<Creature>())
                switch (creature)
                {
                    case Aisling aisling:
                    {
                        var schemas = mapper.MapMany<ItemSchema>(aisling.Inventory);
                        var items = mapper.MapMany<Item>(schemas).ToList();

                        foreach (var item in items)
                        {
                            aisling.Inventory.Remove(item.Slot);
                            aisling.Inventory.TryAdd(item.Slot, item);
                        }
                    }

                        break;

                    case Monster monster:
                    {
                        {
                            var schemas = mapper.MapMany<ItemSchema>(monster.Items);
                            var items = mapper.MapMany<Item>(schemas).ToList();

                            monster.Items.Clear();
                            monster.Items.AddRange(items);
                        }
                    }

                        break;
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
}