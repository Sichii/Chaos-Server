#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class LootTableTests
{
    private static (LootTable Table, Mock<IItemFactory> ItemFactory, Mock<ISimpleCache> Cache) CreateLootTable(
        LootTableMode mode,
        params LootDrop[] drops)
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var cacheMock = new Mock<ISimpleCache>();

        // Default: any item created is non-stackable
        itemFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()))
                       .Returns<string, ICollection<string>>((key, _) => MockItem.Create(key));

        // Default: any template lookup returns non-stackable
        cacheMock.Setup(c => c.Get<ItemTemplate>(It.IsAny<string>()))
                 .Returns<string>(key => new ItemTemplate
                 {
                     Name = key,
                     TemplateKey = key,
                     ItemSprite = new ItemSprite(1, 1),
                     PanelSprite = 1,
                     Color = DisplayColor.Default,
                     PantsColor = DisplayColor.Default,
                     MaxStacks = 1,
                     BuyCost = 0,
                     SellValue = 0,
                     Category = "test",
                     Description = null,
                     EquipmentType = null,
                     Gender = null,
                     Class = null,
                     AdvClass = null,
                     IsDyeable = false,
                     IsModifiable = false,
                     NoTrade = false,
                     AccountBound = false,
                     PreventBanking = false,
                     Level = 1,
                     AbilityLevel = 0,
                     MaxDurability = null,
                     Modifiers = null,
                     Weight = 1,
                     Cooldown = null,
                     RequiresMaster = false,
                     ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                     ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
                 });

        var table = new LootTable(itemFactoryMock.Object, cacheMock.Object, MockScriptProvider.ItemCloner.Object)
        {
            Key = "test_table",
            Mode = mode,
            LootDrops = drops.ToList()
        };

        return (table, itemFactoryMock, cacheMock);
    }

    #region ChancePerItem mode - stackable items
    [Test]
    public void GenerateLoot_ChancePerItem_ShouldSetCountForStackableItems()
    {
        var drop = new LootDrop
        {
            ItemTemplateKey = "potion",
            DropChance = 100m,
            MinAmount = 5,
            MaxAmount = 5
        };

        (var table, var itemFactory, var cache) = CreateLootTable(LootTableMode.ChancePerItem, drop);

        // Override to return stackable template and item
        cache.Setup(c => c.Get<ItemTemplate>("potion"))
             .Returns(
                 new ItemTemplate
                 {
                     Name = "potion",
                     TemplateKey = "potion",
                     ItemSprite = new ItemSprite(1, 1),
                     PanelSprite = 1,
                     Color = DisplayColor.Default,
                     PantsColor = DisplayColor.Default,
                     MaxStacks = 100,
                     BuyCost = 0,
                     SellValue = 0,
                     Category = "test",
                     Description = null,
                     EquipmentType = null,
                     Gender = null,
                     Class = null,
                     AdvClass = null,
                     IsDyeable = false,
                     IsModifiable = false,
                     NoTrade = false,
                     AccountBound = false,
                     PreventBanking = false,
                     Level = 1,
                     AbilityLevel = 0,
                     MaxDurability = null,
                     Modifiers = null,
                     Weight = 1,
                     Cooldown = null,
                     RequiresMaster = false,
                     ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                     ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
                 });

        itemFactory.Setup(f => f.Create("potion", It.IsAny<ICollection<string>>()))
                   .Returns(MockItem.Create("potion", 1, true));

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .NotBeEmpty();

        loot.Sum(i => i.Count)
            .Should()
            .Be(5);
    }
    #endregion

    #region Empty loot table
    [Test]
    public void GenerateLoot_ShouldReturnEmpty_WhenNoLootDrops()
    {
        (var table, _, _) = CreateLootTable(LootTableMode.ChancePerItem);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .BeEmpty();
    }
    #endregion

    #region ChancePerItem mode
    [Test]
    public void GenerateLoot_ChancePerItem_ShouldDropItemsWith100PercentChance()
    {
        var drop = new LootDrop
        {
            ItemTemplateKey = "sword",
            DropChance = 100m,
            MinAmount = 1,
            MaxAmount = 1
        };

        (var table, _, _) = CreateLootTable(LootTableMode.ChancePerItem, drop);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .NotBeEmpty();
    }

    [Test]
    public void GenerateLoot_ChancePerItem_ShouldNotDropItemsWith0PercentChance()
    {
        var drop = new LootDrop
        {
            ItemTemplateKey = "sword",
            DropChance = 0m,
            MinAmount = 1,
            MaxAmount = 1
        };

        (var table, _, _) = CreateLootTable(LootTableMode.ChancePerItem, drop);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .BeEmpty();
    }

    [Test]
    public void GenerateLoot_ChancePerItem_ShouldCreateItemsForNonStackable()
    {
        var drop = new LootDrop
        {
            ItemTemplateKey = "sword",
            DropChance = 100m,
            MinAmount = 1,
            MaxAmount = 1
        };

        (var table, _, _) = CreateLootTable(LootTableMode.ChancePerItem, drop);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .ContainSingle();
    }
    #endregion

    #region PickSingleOrDefault mode
    [Test]
    public void GenerateLoot_PickSingle_ShouldReturnSingleItem()
    {
        var drop = new LootDrop
        {
            ItemTemplateKey = "sword",
            DropChance = 100m,
            MinAmount = 1,
            MaxAmount = 1
        };

        (var table, _, _) = CreateLootTable(LootTableMode.PickSingleOrDefault, drop);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .HaveCount(1);
    }

    [Test]
    public void GenerateLoot_PickSingle_ShouldReturnEmpty_WhenNoDrops()
    {
        (var table, _, _) = CreateLootTable(LootTableMode.PickSingleOrDefault);

        var loot = table.GenerateLoot()
                        .ToList();

        loot.Should()
            .BeEmpty();
    }
    #endregion
}