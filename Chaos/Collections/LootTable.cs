#region
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a table of loot from which items can be generated
/// </summary>
/// <param name="itemFactory">
///     A service used to create items
/// </param>
public sealed class LootTable(IItemFactory itemFactory) : ILootTable
{
    private readonly IItemFactory ItemFactory = itemFactory;

    /// <summary>
    ///     A unique string key identifier for the loot table
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     The loot drops that can be generated from the table
    /// </summary>
    public required ICollection<LootDrop> LootDrops { get; init; } = Array.Empty<LootDrop>();

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
                foreach (var drop in LootDrops)
                    if (DecimalRandomizer.RollChance(drop.DropChance))
                        yield return ItemFactory.Create(drop.ItemTemplateKey);

                break;
            }
            case LootTableMode.PickSingleOrDefault:
            {
                var itemTemplateKey = LootDrops.ToDictionary(drop => drop.ItemTemplateKey, drop => drop.DropChance)
                                               .PickRandomWeightedSingleOrDefault();

                if (itemTemplateKey is not null)
                    yield return ItemFactory.Create(itemTemplateKey);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}