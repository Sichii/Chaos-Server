#region
using Chaos.Collections.Abstractions;
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a loot table that is composed of multiple loot tables
/// </summary>
/// <param name="lootTables">
///     The loot tables contained within this loot table
/// </param>
public sealed class CompositeLootTable(IEnumerable<ILootTable> lootTables) : ILootTable
{
    private readonly ICollection<ILootTable> LootTables = lootTables.ToList();

    /// <inheritdoc />
    public IEnumerable<Item> GenerateLoot() => LootTables.SelectMany(table => table.GenerateLoot());

    /// <summary>
    ///     Gets all loot tables contained within this loot table
    /// </summary>
    /// <remarks>
    ///     If this loot table contains any composite loot tables, this method will recursively return all loot tables
    /// </remarks>
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