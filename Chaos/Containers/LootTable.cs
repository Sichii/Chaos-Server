using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Containers;

public sealed class LootTable
{
    private readonly IItemFactory ItemFactory;
    public LootTable(IItemFactory itemFactory) => ItemFactory = itemFactory;
    public required string Key { get; init; }
    public required ICollection<LootDrop> LootDrops { get; init; } = Array.Empty<LootDrop>();

    public IEnumerable<Item> GenerateLoot()
    {
        foreach (var drop in LootDrops)
            if (Randomizer.RollChance(drop.DropChance))
                yield return ItemFactory.Create(drop.ItemTemplateKey);
    }
}