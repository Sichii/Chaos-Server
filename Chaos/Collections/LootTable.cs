#region
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a table of loot from which items can be generated
/// </summary>
/// <param name="itemFactory">
///     A service used to create items
/// </param>
public sealed class LootTable(IItemFactory itemFactory, ISimpleCache cache, ICloningService<Item> itemCloner) : ILootTable
{
    private readonly ISimpleCache Cache = cache;
    private readonly ICloningService<Item> ItemCloner = itemCloner;
    private readonly IItemFactory ItemFactory = itemFactory;

    /// <summary>
    ///     A unique string key identifier for the loot table
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     The loot drops that can be generated from the table
    /// </summary>
    public required ICollection<LootDrop> LootDrops { get; init; } = [];

    /// <summary>
    ///     The mode in which the loot table operates
    /// </summary>
    public required LootTableMode Mode { get; init; }

    /// <inheritdoc />
    public IEnumerable<Item> GenerateLoot()
    {
        switch (Mode)
        {
            case LootTableMode.ChancePerItem:
            {
                return LootDrops.Where(drop => DecimalRandomizer.RollChance(drop.DropChance))
                                .SelectMany(InnerGenerateLoot)
                                .FixStacks(ItemCloner);
            }
            case LootTableMode.PickSingleOrDefault:
            {
                var drop = LootDrops.ToDictionary(drop => drop, drop => drop.DropChance)
                                    .PickRandomWeightedSingleOrDefault();

                if (drop is not null)
                    return InnerGenerateLoot(drop);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return [];

        IEnumerable<Item> InnerGenerateLoot(LootDrop localDrop)
        {
            var amount = Random.Shared.Next(localDrop.MinAmount, localDrop.MaxAmount + 1);
            var template = Cache.Get<ItemTemplate>(localDrop.ItemTemplateKey);

            if (template.Stackable)
            {
                var item = ItemFactory.Create(localDrop.ItemTemplateKey, localDrop.ExtraScriptKeys);
                item.Count = amount;

                return item.FixStacks(ItemCloner);
            }

            return Enumerable.Repeat(ItemFactory.Create(localDrop.ItemTemplateKey, localDrop.ExtraScriptKeys), amount);
        }
    }
}