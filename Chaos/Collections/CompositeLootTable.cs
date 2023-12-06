using Chaos.Collections.Abstractions;
using Chaos.Models.Panel;

namespace Chaos.Collections;

public sealed class CompositeLootTable(IEnumerable<ILootTable> lootTables) : ILootTable
{
    private readonly ICollection<ILootTable> LootTables = lootTables.ToList();

    /// <inheritdoc />
    public IEnumerable<Item> GenerateLoot() => LootTables.SelectMany(table => table.GenerateLoot());

    public IEnumerable<LootTable> GetLootTables()
    {
        foreach (var lootTable in LootTables)
            switch (lootTable)
            {
                case CompositeLootTable composite:
                {
                    foreach (var subTable in composite.GetLootTables())
                        yield return subTable;

                    break;
                }
                case LootTable table:
                    yield return table;

                    break;
            }
    }
}