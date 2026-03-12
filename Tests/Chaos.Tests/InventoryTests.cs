#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class InventoryTests
{
    #region Constructor with items
    [Test]
    public void Constructor_WithItems_ShouldPopulateSlots()
    {
        var item1 = MockItem.Create("Sword");
        item1.Slot = 1;

        var item2 = MockItem.Create("Shield");
        item2.Slot = 5;

        var inventory = new Inventory(
            [
                item1,
                item2
            ]);

        inventory.Contains("Sword")
                 .Should()
                 .BeTrue();

        inventory.Contains("Shield")
                 .Should()
                 .BeTrue();
    }
    #endregion

    private Inventory CreateInventory() => new(MockScriptProvider.ItemCloner.Object);

    #region Full inventory
    [Test]
    public void TryAddToNextSlot_ShouldReturnFalse_WhenInventoryIsFull()
    {
        var inventory = CreateInventory();

        // Inventory has 60 slots (indices 1-59, slot 0 is invalid)
        for (byte i = 1; i < 60; i++)
            inventory.TryAddDirect(i, MockItem.Create($"Item{i}"));

        inventory.TryAddToNextSlot(MockItem.Create("Extra"))
                 .Should()
                 .BeFalse();
    }
    #endregion

    #region TrySwap — partial merge
    [Test]
    public void TrySwap_ShouldPartiallyMerge_WhenSourceHasMoreThanDestinationCanAccept()
    {
        var inventory = CreateInventory();
        var item1 = MockItem.Create("Potion", 80, true);
        var item2 = MockItem.Create("Potion", 50, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        // MaxStacks=100. item2 can accept 50 more. item1 has 80. stacksToGive = min(50, 80) = 50
        // stacksToGive(50) != item1.Count(80) → partial merge, item1 retains 30
        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        item2.Count
             .Should()
             .Be(100);

        item1.Count
             .Should()
             .Be(30);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(130);
    }
    #endregion

    #region Contains
    [Test]
    public void Contains_ByName_ShouldReturnTrue_WhenItemExists()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");
        inventory.TryAddToNextSlot(item);

        inventory.Contains("Sword")
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void Contains_ByName_ShouldBeCaseInsensitive()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        inventory.Contains("sword")
                 .Should()
                 .BeTrue();

        inventory.Contains("SWORD")
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void Contains_ByName_ShouldReturnFalse_WhenItemNotPresent()
    {
        var inventory = CreateInventory();

        inventory.Contains("Sword")
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void Contains_ByItem_ShouldDelegateToNameMatch()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");
        inventory.TryAddToNextSlot(item);

        var otherItem = MockItem.Create("Sword");

        inventory.Contains(otherItem)
                 .Should()
                 .BeTrue();
    }
    #endregion

    #region CountOf / HasCount
    [Test]
    public void CountOf_ShouldReturnZero_WhenItemNotPresent()
    {
        var inventory = CreateInventory();

        inventory.CountOf("Nonexistent")
                 .Should()
                 .Be(0);
    }

    [Test]
    public void CountOf_ShouldReturnItemCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.CountOf("Potion")
                 .Should()
                 .Be(5);
    }

    [Test]
    public void CountOf_ShouldSumAcrossMultipleStacks()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 50, true));
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 30, true));

        inventory.CountOf("Potion")
                 .Should()
                 .Be(80);
    }

    [Test]
    public void CountOfByTemplateKey_ShouldMatchByTemplateKey()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.CountOfByTemplateKey("potion")
                 .Should()
                 .Be(10);
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenSufficientCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.HasCount("Potion", 5)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenExactCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.HasCount("Potion", 5)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnFalse_WhenInsufficientCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.HasCount("Potion", 5)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void HasCountByTemplateKey_ShouldReturnTrue_WhenSufficientCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.HasCountByTemplateKey("potion", 5)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void HasCountByTemplateKey_ShouldReturnFalse_WhenInsufficientCount()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.HasCountByTemplateKey("potion", 5)
                 .Should()
                 .BeFalse();
    }
    #endregion

    #region RemoveQuantity(name, qty)
    [Test]
    public void RemoveQuantity_ByName_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantity("Potion", 0)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldReturnFalse_WhenQuantityIsNegative()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantity("Potion", -1)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldReturnFalse_WhenItemNotFound()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantity("Nonexistent", 1)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldReturnFalse_WhenInsufficientQuantity()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.RemoveQuantity("Potion", 10)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldRemoveEntireStack_WhenCountMatchesQuantity()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantity("Potion", 5)
                 .Should()
                 .BeTrue();

        inventory.Contains("Potion")
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldReduceCount_WhenPartialRemoval()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.RemoveQuantity("Potion", 3)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(7);
    }

    [Test]
    public void RemoveQuantity_ByName_ShouldRemoveAcrossStacks()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);

        // Add to separate slots to prevent stacking consolidation
        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.RemoveQuantity("Potion", 5)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(1);
    }
    #endregion

    #region RemoveQuantity(name, qty, out items)
    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantity("Potion", 0, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldReturnFalse_WhenItemNotFound()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantity("Nonexistent", 1, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldReturnFalse_WhenInsufficientQuantity()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.RemoveQuantity("Potion", 10, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldReturnRemovedItems()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantity("Potion", 5, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .NotBeNull();

        items.Sum(i => i.Count)
             .Should()
             .Be(5);
    }

    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldSplitStack_WhenPartialRemoval()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.RemoveQuantity("Potion", 3, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .NotBeNull();

        items.Sum(i => i.Count)
             .Should()
             .Be(3);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(7);
    }
    #endregion

    #region RemoveQuantity(slot, qty)
    [Test]
    public void RemoveQuantity_BySlot_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 5, true);
        inventory.TryAddToNextSlot(item);

        inventory.RemoveQuantity(item.Slot, 0)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_BySlot_ShouldReturnFalse_WhenSlotIsEmpty()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantity(1, 1)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_BySlot_ShouldReturnFalse_WhenInsufficientQuantity()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 3, true);
        inventory.TryAddToNextSlot(item);

        inventory.RemoveQuantity(item.Slot, 10)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantity_BySlot_ShouldRemoveFromSpecifiedSlotFirst()
    {
        var inventory = CreateInventory();
        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 5, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.RemoveQuantity(2, 7)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(1);
    }
    #endregion

    #region RemoveQuantity(slot, qty, out items)
    [Test]
    public void RemoveQuantity_BySlotWithItems_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 5, true);
        inventory.TryAddToNextSlot(item);

        inventory.RemoveQuantity(item.Slot, 0, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantity_BySlotWithItems_ShouldReturnFalse_WhenSlotIsEmpty()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantity(1, 1, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantity_BySlotWithItems_ShouldReturnSplitItems()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 10, true);
        inventory.TryAddToNextSlot(item);

        inventory.RemoveQuantity(item.Slot, 3, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .NotBeNull();

        items.Sum(i => i.Count)
             .Should()
             .Be(3);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(7);
    }
    #endregion

    #region RemoveQuantityByTemplateKey
    [Test]
    public void RemoveQuantityByTemplateKey_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantityByTemplateKey("potion", 0)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantityByTemplateKey_ShouldReturnFalse_WhenItemNotFound()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantityByTemplateKey("nonexistent", 1)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantityByTemplateKey_ShouldReturnFalse_WhenInsufficientQuantity()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.RemoveQuantityByTemplateKey("potion", 10)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void RemoveQuantityByTemplateKey_ShouldRemoveMatchingItems()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.RemoveQuantityByTemplateKey("potion", 5)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(5);
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldReturnRemovedItems()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 10, true));

        inventory.RemoveQuantityByTemplateKey("potion", 5, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .NotBeNull();

        items.Sum(i => i.Count)
             .Should()
             .Be(5);
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldReturnFalse_WhenQuantityIsZero()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        inventory.RemoveQuantityByTemplateKey("potion", 0, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldReturnFalse_WhenNotFound()
    {
        var inventory = CreateInventory();

        inventory.RemoveQuantityByTemplateKey("nonexistent", 1, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldReturnFalse_WhenInsufficient()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        inventory.RemoveQuantityByTemplateKey("potion", 10, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }
    #endregion

    #region TryAdd / TryAddToNextSlot
    [Test]
    public void TryAdd_ShouldAddNonStackableToSlot()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");

        inventory.TryAdd(1, item)
                 .Should()
                 .BeTrue();

        inventory.Contains("Sword")
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void TryAdd_ShouldStackExistingItem_WhenStackable()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));

        var newPotion = MockItem.Create("Potion", 3, true);

        inventory.TryAdd(5, newPotion)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(8);
    }

    [Test]
    public void TryAdd_ShouldFallBackToNextSlot_WhenStackingFails()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");

        inventory.TryAdd(1, item)
                 .Should()
                 .BeTrue();

        // Different non-stackable item can't go in same slot
        var item2 = MockItem.Create("Shield");

        inventory.TryAdd(1, item2)
                 .Should()
                 .BeTrue();

        inventory.Contains("Shield")
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void TryAddToNextSlot_ShouldConsolidateStackables()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 5, true));
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 3, true));

        // Should consolidate into one stack
        inventory.CountOf("Potion")
                 .Should()
                 .Be(8);
    }

    [Test]
    public void TryAddToNextSlot_ShouldAddNonStackableToFirstAvailableSlot()
    {
        var inventory = CreateInventory();

        inventory.TryAddToNextSlot(MockItem.Create("Sword"))
                 .Should()
                 .BeTrue();

        inventory.TryAddToNextSlot(MockItem.Create("Shield"))
                 .Should()
                 .BeTrue();

        inventory.Contains("Sword")
                 .Should()
                 .BeTrue();

        inventory.Contains("Shield")
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void TryAddDirect_ShouldAddToExactSlot()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");

        inventory.TryAddDirect(5, item)
                 .Should()
                 .BeTrue();

        item.Slot
            .Should()
            .Be(5);
    }

    [Test]
    public void TryAddDirect_ShouldReturnFalse_WhenSlotOccupied()
    {
        var inventory = CreateInventory();
        inventory.TryAddDirect(5, MockItem.Create("Sword"));

        inventory.TryAddDirect(5, MockItem.Create("Shield"))
                 .Should()
                 .BeFalse();
    }
    #endregion

    #region TryAddStackable behavior
    [Test]
    public void TryAddStackable_ShouldNormalizeZeroCountToOne()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 0, true);

        inventory.TryAddToNextSlot(item)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(1);
    }

    [Test]
    public void TryAddStackable_ShouldOverflow_WhenStackIsFull()
    {
        var inventory = CreateInventory();

        // MaxStacks for stackable is 100
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 90, true));

        // This should partially fill existing stack and overflow to new stack
        inventory.TryAddToNextSlot(MockItem.Create("Potion", 20, true));

        inventory.CountOf("Potion")
                 .Should()
                 .Be(110);
    }

    [Test]
    public void TryAdd_StackableWithPreferredSlot_ShouldPreferSpecifiedSlot()
    {
        var inventory = CreateInventory();
        var existing = MockItem.Create("Potion", 5, true);
        inventory.TryAddDirect(3, existing);

        var newItem = MockItem.Create("Potion", 3, true);

        // TryAdd(slot, obj) with stackable should try stacking at preferred slot first
        inventory.TryAdd(3, newItem)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(8);
    }
    #endregion

    #region TrySwap
    [Test]
    public void TrySwap_ShouldSwapNonStackableItems()
    {
        var inventory = CreateInventory();
        var sword = MockItem.Create("Sword");
        var shield = MockItem.Create("Shield");

        inventory.TryAddDirect(1, sword);
        inventory.TryAddDirect(2, shield);

        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        sword.Slot
             .Should()
             .Be(2);

        shield.Slot
              .Should()
              .Be(1);
    }

    [Test]
    public void TrySwap_ShouldMergeStackables_WhenSameName()
    {
        var inventory = CreateInventory();
        var item1 = MockItem.Create("Potion", 30, true);
        var item2 = MockItem.Create("Potion", 20, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        // item2 should gain stacks from item1
        inventory.CountOf("Potion")
                 .Should()
                 .Be(50);
    }

    [Test]
    public void TrySwap_ShouldNotMerge_WhenDifferentNames()
    {
        var inventory = CreateInventory();
        var item1 = MockItem.Create("HealthPotion", 5, true);
        var item2 = MockItem.Create("ManaPotion", 5, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        // Should do a regular swap
        item1.Slot
             .Should()
             .Be(2);

        item2.Slot
             .Should()
             .Be(1);
    }

    [Test]
    public void TrySwap_ShouldNotMerge_WhenEitherIsAtMaxStacks()
    {
        var inventory = CreateInventory();

        // MaxStacks is 100 for stackable
        var item1 = MockItem.Create("Potion", 100, true);
        var item2 = MockItem.Create("Potion", 50, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        // Should do regular swap because item1 is at max stacks
        item1.Slot
             .Should()
             .Be(2);

        item2.Slot
             .Should()
             .Be(1);
    }

    [Test]
    public void TrySwap_ShouldRemoveSource_WhenFullyMerged()
    {
        var inventory = CreateInventory();
        var item1 = MockItem.Create("Potion", 10, true);
        var item2 = MockItem.Create("Potion", 50, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        inventory.TrySwap(1, 2)
                 .Should()
                 .BeTrue();

        // item1 should be fully merged into item2, so slot 1 should be empty
        inventory.TryGetObject(1, out _)
                 .Should()
                 .BeFalse();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(60);
    }
    #endregion

    #region RemoveQuantity — early break across stacks
    [Test]
    public void RemoveQuantity_ByName_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        // Remove 6 = first stack (3) + second stack (3), then quantity=0, should break before third
        inventory.RemoveQuantity("Potion", 6)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantity_BySlot_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        // Remove 6 starting from slot 1, consumes slot 1 (3) + slot 2 (3) → qty=0, breaks before slot 3
        inventory.RemoveQuantity(1, 6)
                 .Should()
                 .BeTrue();

        inventory.CountOf("Potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantity_ByNameWithItems_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        inventory.RemoveQuantity("Potion", 6, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .HaveCount(2);

        items.Sum(i => i.Count)
             .Should()
             .Be(6);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantity_BySlotWithItems_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        inventory.RemoveQuantity(1, 6, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .HaveCount(2);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantity_BySlotWithItems_ShouldReturnFalse_WhenInsufficientCount()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Potion", 3, true);
        inventory.TryAddToNextSlot(item);

        inventory.RemoveQuantity(item.Slot, 10, out var items)
                 .Should()
                 .BeFalse();

        items.Should()
             .BeNull();
    }
    #endregion

    #region RemoveQuantityByTemplateKey — cross-stack branches
    [Test]
    public void RemoveQuantityByTemplateKey_ShouldRemoveAcrossStacks_AndPartialSplit()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 5, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        // Remove 5: first stack fully removed (3 <= 5, qty→2), second stack split (5 > 2)
        inventory.RemoveQuantityByTemplateKey("potion", 5)
                 .Should()
                 .BeTrue();

        inventory.CountOfByTemplateKey("potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldRemoveAcrossStacks_AndPartialSplit()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 5, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);

        // Remove 5: first stack fully removed (3, qty→2), second stack split
        inventory.RemoveQuantityByTemplateKey("potion", 5, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .NotBeNull();

        items.Sum(i => i.Count)
             .Should()
             .Be(5);

        inventory.CountOfByTemplateKey("potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantityByTemplateKey_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        inventory.RemoveQuantityByTemplateKey("potion", 6)
                 .Should()
                 .BeTrue();

        inventory.CountOfByTemplateKey("potion")
                 .Should()
                 .Be(3);
    }

    [Test]
    public void RemoveQuantityByTemplateKey_WithItems_ShouldBreakEarly_WhenQuantityReachesZeroMidLoop()
    {
        var inventory = CreateInventory();

        var item1 = MockItem.Create("Potion", 3, true);
        var item2 = MockItem.Create("Potion", 3, true);
        var item3 = MockItem.Create("Potion", 3, true);

        inventory.TryAddDirect(1, item1);
        inventory.TryAddDirect(2, item2);
        inventory.TryAddDirect(3, item3);

        inventory.RemoveQuantityByTemplateKey("potion", 6, out var items)
                 .Should()
                 .BeTrue();

        items.Should()
             .HaveCount(2);

        inventory.CountOfByTemplateKey("potion")
                 .Should()
                 .Be(3);
    }
    #endregion

    #region TryAddStackable edge cases
    [Test]
    public void TryAdd_ShouldIgnoreInvalidPreferredSlot_WhenSlotIsZero()
    {
        var inventory = CreateInventory();
        var existing = MockItem.Create("Potion", 5, true);
        inventory.TryAddDirect(1, existing);

        var newItem = MockItem.Create("Potion", 3, true);

        // Slot 0 is invalid for Inventory — preferredSlot should be reset to null
        inventory.TryAdd(0, newItem)
                 .Should()
                 .BeTrue();

        // Should still consolidate into existing stack since preferredSlot was nulled
        inventory.CountOf("Potion")
                 .Should()
                 .Be(8);
    }

    [Test]
    public void TryAddStackable_ShouldSkipStack_WhenAtMaxStacks()
    {
        var inventory = CreateInventory();

        // Stack at MaxStacks (100)
        var fullStack = MockItem.Create("Potion", 100, true);
        inventory.TryAddDirect(1, fullStack);

        // Adding more should overflow to a new slot since the existing stack is full
        var newItem = MockItem.Create("Potion", 5, true);

        inventory.TryAddToNextSlot(newItem)
                 .Should()
                 .BeTrue();

        // fullStack should remain at 100, new stack should be in a new slot
        fullStack.Count
                 .Should()
                 .Be(100);

        inventory.CountOf("Potion")
                 .Should()
                 .Be(105);
    }
    #endregion

    #region Indexer / TryGetObject / TryGetRemove / Remove(name)
    [Test]
    public void Indexer_ByName_ShouldReturnItem_WhenFound()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");
        inventory.TryAddToNextSlot(item);

        inventory["Sword"]
            .Should()
            .BeSameAs(item);
    }

    [Test]
    public void Indexer_ByName_ShouldReturnNull_WhenNotFound()
    {
        var inventory = CreateInventory();

        inventory["Nonexistent"]
            .Should()
            .BeNull();
    }

    [Test]
    public void Indexer_ByName_ShouldBeCaseInsensitive()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        inventory["sword"]
            .Should()
            .NotBeNull();
    }

    [Test]
    public void TryGetObject_ByName_ShouldReturnTrue_WhenFound()
    {
        var inventory = CreateInventory();
        var item = MockItem.Create("Sword");
        inventory.TryAddToNextSlot(item);

        inventory.TryGetObject("Sword", out var found)
                 .Should()
                 .BeTrue();

        found.Should()
             .BeSameAs(item);
    }

    [Test]
    public void TryGetObject_ByName_ShouldReturnFalse_WhenNotFound()
    {
        var inventory = CreateInventory();

        inventory.TryGetObject("Nonexistent", out var found)
                 .Should()
                 .BeFalse();

        found.Should()
             .BeNull();
    }

    [Test]
    public void TryGetRemove_ShouldRemoveItem_WhenFound()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        inventory.TryGetRemove("Sword", out var removed)
                 .Should()
                 .BeTrue();

        removed.Should()
               .NotBeNull();

        inventory.Contains("Sword")
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void TryGetRemove_ShouldReturnFalse_WhenNotFound()
    {
        var inventory = CreateInventory();

        inventory.TryGetRemove("Nonexistent", out var removed)
                 .Should()
                 .BeFalse();

        removed.Should()
               .BeNull();
    }

    [Test]
    public void Remove_ByName_ShouldReturnTrue_WhenFound()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        inventory.Remove("Sword")
                 .Should()
                 .BeTrue();

        inventory.Contains("Sword")
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void Remove_ByName_ShouldReturnFalse_WhenNotFound()
    {
        var inventory = CreateInventory();

        inventory.Remove("Nonexistent")
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void Remove_ByName_ShouldBeCaseInsensitive()
    {
        var inventory = CreateInventory();
        inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        inventory.Remove("sword")
                 .Should()
                 .BeTrue();
    }
    #endregion
}