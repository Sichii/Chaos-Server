using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Panel;

namespace Chaos.Containers;

public sealed class LootTable
{
    private readonly IItemFactory ItemFactory;
    public required string Key { get; init; }
    public required ICollection<LootDrop> LootDrops { get; init; } = Array.Empty<LootDrop>();
    public LootTable(IItemFactory itemFactory) => ItemFactory = itemFactory;

    public IEnumerable<Item> GenerateLoot()
    {
        foreach (var drop in LootDrops)
            if (Randomizer.RollChance(drop.DropChance))
                yield return ItemFactory.Create(drop.ItemTemplateKey);
    }
}