#region
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Utilities;

public static class ComplexActionHelper
{
    public enum AddManyItemsResult
    {
        Success,
        CantCarry,
        BadInput
    }

    public enum BuyItemResult
    {
        Success,
        CantCarry,
        NotEnoughGold,
        BadInput,
        NotEnoughStock
    }

    public enum DepositGoldResult
    {
        Success,
        DontHaveThatMany,
        BadInput
    }

    public enum DepositItemResult
    {
        Success,
        DontHaveThatMany,
        NotEnoughGold,
        BadInput,
        ItemDamaged
    }

    public enum LearnSkillResult
    {
        Success,
        NoRoom
    }

    public enum LearnSpellResult
    {
        Success,
        NoRoom
    }

    public enum RemoveManyItemsResult
    {
        Success,
        DontHaveThatMany,
        BadInput
    }

    public enum SellItemResult
    {
        Success,
        DontHaveThatMany,
        TooMuchGold,
        BadInput,
        ItemDamaged
    }

    public enum WithdrawGoldResult
    {
        Success,
        TooMuchGold,
        DontHaveThatMany,
        BadInput
    }

    public enum WithdrawItemResult
    {
        Success,
        CantCarry,
        DontHaveThatMany,
        BadInput
    }

    public static BuyItemResult BuyItem(
        Aisling source,
        IBuyShopSource? shop,
        Item fauxItem,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        int amount,
        int costPerItem)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(fauxItem);
        ArgumentNullException.ThrowIfNull(itemFactory);
        ArgumentNullException.ThrowIfNull(itemCloner);

        if (amount < 1)
            return BuyItemResult.BadInput;

        if (costPerItem < 0)
            return BuyItemResult.BadInput;

        //fail fast
        if ((shop != null) && (shop.GetStock(fauxItem.Template.TemplateKey) < amount))
            return BuyItemResult.NotEnoughStock;

        if (!source.CanCarry((fauxItem, amount)))
            return BuyItemResult.CantCarry;

        var totalCost = costPerItem * amount;

        if ((shop != null) && !shop.TryDecrementStock(fauxItem.Template.TemplateKey, amount))
            return BuyItemResult.NotEnoughStock;

        if (!source.TryTakeGold(totalCost))
        {
            //add the stock back if we fail to take the gold
            shop?.TryDecrementStock(fauxItem.Template.TemplateKey, -amount);

            return BuyItemResult.NotEnoughGold;
        }

        var stackedItem = itemFactory.Create(fauxItem.Template.TemplateKey, fauxItem.ScriptKeys);
        stackedItem.Count = amount;

        foreach (var item in stackedItem.FixStacks(itemCloner))
            source.Inventory.TryAddToNextSlot(item);

        return BuyItemResult.Success;
    }

    public static DepositGoldResult DepositGold(Aisling source, int amount)
    {
        if (amount < 0)
            return DepositGoldResult.BadInput;

        if (!source.TryTakeGold(amount))
            return DepositGoldResult.DontHaveThatMany;

        source.Bank.AddGold((uint)amount);

        return DepositGoldResult.Success;
    }

    public static DepositItemResult DepositItem(
        Aisling source,
        byte slot,
        int amount,
        int costPerItem = 0)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (amount < 1)
            return DepositItemResult.BadInput;

        if (costPerItem < 0)
            return DepositItemResult.BadInput;

        var slotItem = source.Inventory[slot];

        if (slotItem == null)
            return DepositItemResult.BadInput;

        if (slotItem.PreventBanking)
            return DepositItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return DepositItemResult.DontHaveThatMany;

        if (slotItem.CurrentDurability != slotItem.Template.MaxDurability)
            return DepositItemResult.ItemDamaged;

        var totalCost = amount * costPerItem;

        if (!source.TryTakeGold(totalCost))
            return DepositItemResult.NotEnoughGold;

        source.Inventory.RemoveQuantity(slot, amount, out var items);

        //if any item is damaged, return it to the inventory
        //and return the result
        if (items!.Any(i => i.CurrentDurability != i.Template.MaxDurability))
        {
            foreach (var item in items!)
                source.Inventory.TryAdd(item.Slot, item);

            return DepositItemResult.ItemDamaged;
        }

        foreach (var item in items!)
            source.Bank.Deposit(item);

        return DepositItemResult.Success;
    }

    public static DepositItemResult DepositItem(
        Aisling source,
        string itemName,
        int amount,
        int costPerItem = 0)
    {
        if (amount < 1)
            return DepositItemResult.BadInput;

        if (costPerItem < 0)
            return DepositItemResult.BadInput;

        var slotItem = source.Inventory[itemName];

        if (slotItem == null)
            return DepositItemResult.BadInput;

        if (slotItem.PreventBanking)
            return DepositItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return DepositItemResult.DontHaveThatMany;

        if (slotItem.CurrentDurability != slotItem.Template.MaxDurability)
            return DepositItemResult.ItemDamaged;

        var totalCost = amount * costPerItem;

        if (!source.TryTakeGold(totalCost))
            return DepositItemResult.NotEnoughGold;

        source.Inventory.RemoveQuantity(itemName, amount, out var items);

        //if any item is damaged, return it to the inventory
        //and return the result
        if (items!.Any(i => i.CurrentDurability != i.Template.MaxDurability))
        {
            foreach (var item in items!)
                source.Inventory.TryAdd(item.Slot, item);

            return DepositItemResult.ItemDamaged;
        }

        foreach (var item in items!)
            source.Bank.Deposit(item);

        return DepositItemResult.Success;
    }

    public static LearnSkillResult LearnSkill(Aisling source, Skill skill)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(skill);

        if (source.SkillBook.AvailableSlots == 0)
            return LearnSkillResult.NoRoom;

        if (source.SkillBook.TryAddToNextSlot(skill))
            return LearnSkillResult.Success;

        return LearnSkillResult.NoRoom;
    }

    public static LearnSpellResult LearnSpell(Aisling source, Spell spell)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(spell);

        if (source.SpellBook.AvailableSlots == 0)
            return LearnSpellResult.NoRoom;

        if (source.SpellBook.TryAddToNextSlot(spell))
            return LearnSpellResult.Success;

        return LearnSpellResult.NoRoom;
    }

    public static RemoveManyItemsResult RemoveManyItems(
        Aisling source,
        params IReadOnlyCollection<(string ItemNameOrTemplateKey, int Amount)> items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
            return RemoveManyItemsResult.Success;

        var groupedItems = items.GroupBy(item => item.ItemNameOrTemplateKey)
                                .Select(group => (ItemNameOrTemplateKey: group.Key, Amount: group.Sum(item => item.Amount)))
                                .ToList();

        if (groupedItems.Any(item => !source.Inventory.HasCount(item.ItemNameOrTemplateKey, item.Amount)))
            return RemoveManyItemsResult.DontHaveThatMany;

        foreach (var item in groupedItems)
            source.Inventory.RemoveQuantity(item.ItemNameOrTemplateKey, item.Amount);

        return RemoveManyItemsResult.Success;
    }

    public static RemoveManyItemsResult RemoveManyItems(Aisling source, params IEnumerable<Item> items)
        => RemoveManyItems(
            source,
            items.Select(item => (item.DisplayName, item.Count))
                 .ToArray());

    public static SellItemResult SellItem(
        Aisling source,
        byte slot,
        int amount,
        int valuePerItem)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (amount < 1)
            return SellItemResult.BadInput;

        if (valuePerItem < 0)
            return SellItemResult.BadInput;

        var slotItem = source.Inventory[slot];

        if (slotItem == null)
            return SellItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return SellItemResult.DontHaveThatMany;

        if (slotItem.CurrentDurability != slotItem.Template.MaxDurability)
            return SellItemResult.ItemDamaged;

        var totalValue = valuePerItem * amount;

        if (!source.TryGiveGold(totalValue))
            return SellItemResult.TooMuchGold;

        source.Inventory.RemoveQuantity(slot, amount, out var items);

        if (items!.Any(item => item.CurrentDurability != item.Template.MaxDurability))
        {
            foreach (var item in items!)
                source.Inventory.TryAdd(item.Slot, item);

            source.TryTakeGold(totalValue);

            return SellItemResult.ItemDamaged;
        }

        return SellItemResult.Success;
    }

    public static SellItemResult SellItem(
        Aisling source,
        string itemName,
        int amount,
        int valuePerItem)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(itemName);

        if (amount < 1)
            return SellItemResult.BadInput;

        if (valuePerItem < 0)
            return SellItemResult.BadInput;

        var slotItem = source.Inventory[itemName];

        if (slotItem == null)
            return SellItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return SellItemResult.DontHaveThatMany;

        if (slotItem.CurrentDurability != slotItem.Template.MaxDurability)
            return SellItemResult.ItemDamaged;

        var totalValue = valuePerItem * amount;

        if (!source.TryGiveGold(totalValue))
            return SellItemResult.TooMuchGold;

        source.Inventory.RemoveQuantity(itemName, amount, out var items);

        if (items!.Any(item => item.CurrentDurability != item.Template.MaxDurability))
        {
            foreach (var item in items!)
                source.Inventory.TryAdd(item.Slot, item);

            source.TryTakeGold(totalValue);

            return SellItemResult.ItemDamaged;
        }

        return SellItemResult.Success;
    }

    public static WithdrawGoldResult WithdrawGold(Aisling source, int amount)
    {
        if (amount < 0)
            return WithdrawGoldResult.BadInput;

        if (source.Bank.Gold < amount)
            return WithdrawGoldResult.DontHaveThatMany;

        if (!source.TryGiveGold(amount))
            return WithdrawGoldResult.TooMuchGold;

        source.Bank.RemoveGold((uint)amount);

        return WithdrawGoldResult.Success;
    }

    public static WithdrawItemResult WithdrawItem(Aisling source, string itemName, int amount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(itemName);

        if (amount < 1)
            return WithdrawItemResult.BadInput;

        if (!source.Bank.HasCount(itemName, amount))
            return WithdrawItemResult.DontHaveThatMany;

        var bankItem = source.Bank.First(i => i.DisplayName.EqualsI(itemName));

        if (!source.CanCarry((bankItem, amount)))
            return WithdrawItemResult.CantCarry;

        source.Bank.TryWithdraw(itemName, amount, out var items);

        foreach (var item in items!)
            source.Inventory.TryAddToNextSlot(item);

        return WithdrawItemResult.Success;
    }
}