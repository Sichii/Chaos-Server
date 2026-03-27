#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class CompositeLootTableTests
{
    #region GetLootTables
    [Test]
    public void GetLootTables_ShouldFlattenNestedCompositeTables()
    {
        var itemFactory = new Mock<IItemFactory>();
        var cache = new Mock<ISimpleCache>();

        var innerTable = new LootTable(itemFactory.Object, cache.Object, MockScriptProvider.ItemCloner.Object)
        {
            Key = "inner",
            Mode = LootTableMode.ChancePerItem,
            LootDrops = []
        };

        var outerTable = new LootTable(itemFactory.Object, cache.Object, MockScriptProvider.ItemCloner.Object)
        {
            Key = "outer",
            Mode = LootTableMode.ChancePerItem,
            LootDrops = []
        };

        var innerComposite = new CompositeLootTable([innerTable]);

        var outerComposite = new CompositeLootTable(
            [
                innerComposite,
                outerTable
            ]);

        var tables = outerComposite.GetLootTables()
                                   .ToList();

        tables.Should()
              .HaveCount(2);

        tables.Select(t => t.Key)
              .Should()
              .BeEquivalentTo("inner", "outer");
    }
    #endregion

    #region GenerateLoot
    [Test]
    public void GenerateLoot_ShouldAggregateFromMultipleTables()
    {
        var table1 = new Mock<ILootTable>();
        var table2 = new Mock<ILootTable>();

        table1.Setup(t => t.GenerateLoot())
              .Returns([MockItem.Create("Sword")]);

        table2.Setup(t => t.GenerateLoot())
              .Returns([MockItem.Create("Shield")]);

        var composite = new CompositeLootTable(
            [
                table1.Object,
                table2.Object
            ]);

        var loot = composite.GenerateLoot()
                            .ToList();

        loot.Should()
            .HaveCount(2);

        loot.Select(i => i.DisplayName)
            .Should()
            .BeEquivalentTo("Sword", "Shield");
    }

    [Test]
    public void GenerateLoot_ShouldReturnEmpty_WhenNoTables()
    {
        var composite = new CompositeLootTable([]);

        var loot = composite.GenerateLoot()
                            .ToList();

        loot.Should()
            .BeEmpty();
    }

    [Test]
    public void GenerateLoot_ShouldReturnEmpty_WhenAllTablesReturnEmpty()
    {
        var table = new Mock<ILootTable>();

        table.Setup(t => t.GenerateLoot())
             .Returns([]);

        var composite = new CompositeLootTable([table.Object]);

        composite.GenerateLoot()
                 .ToList()
                 .Should()
                 .BeEmpty();
    }
    #endregion
}