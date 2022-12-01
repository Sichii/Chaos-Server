using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Utilities;

public static class ComplexActionHelper
{
    public enum BuyItemResult
    {
        Success,
        CantCarry,
        NotEnoughGold,
        BadInput
    }

    public enum DepositGoldResult
    {
        Success,
        NotEnoughGold,
        BadInput
    }

    public enum DepositItemResult
    {
        Success,
        DontHaveThatMany,
        NotEnoughGold,
        BadInput
    }

    public enum SellItemResult
    {
        Success,
        DontHaveThatMany,
        TooMuchGold,
        BadInput
    }

    public enum WithdrawGoldResult
    {
        Success,
        TooMuchGold,
        NotEnoughGold,
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
        Item fauxItem,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        int amount,
        int costPerItem
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(fauxItem);
        ArgumentNullException.ThrowIfNull(itemFactory);
        ArgumentNullException.ThrowIfNull(itemCloner);

        if (amount < 1)
            return BuyItemResult.BadInput;

        if (costPerItem < 0)
            return BuyItemResult.BadInput;

        if (!source.CanCarry((fauxItem, amount)))
            return BuyItemResult.CantCarry;

        var totalCost = costPerItem * amount;

        if (!source.TryTakeGold(totalCost))
            return BuyItemResult.NotEnoughGold;

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
            return DepositGoldResult.NotEnoughGold;

        source.Bank.AddGold((uint)amount);

        return DepositGoldResult.Success;
    }

    public static DepositItemResult DepositItem(
        Aisling source,
        byte slot,
        int amount,
        int costPerItem = 0
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        if (amount < 1)
            return DepositItemResult.BadInput;

        if (costPerItem < 0)
            return DepositItemResult.BadInput;

        var slotItem = source.Inventory[slot];

        if (slotItem == null)
            return DepositItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return DepositItemResult.DontHaveThatMany;

        var totalCost = amount * costPerItem;

        if (!source.TryTakeGold(totalCost))
            return DepositItemResult.NotEnoughGold;

        source.Inventory.RemoveQuantity(slot, amount, out var items);

        foreach (var item in items!)
            source.Bank.Deposit(item);

        return DepositItemResult.Success;
    }

    public static DepositItemResult DepositItem(
        Aisling source,
        string itemName,
        int amount,
        int costPerItem = 0
    )
    {
        if (amount < 1)
            return DepositItemResult.BadInput;

        if (costPerItem < 0)
            return DepositItemResult.BadInput;

        var slotItem = source.Inventory[itemName];

        if (slotItem == null)
            return DepositItemResult.BadInput;

        if (!source.Inventory.HasCount(slotItem.DisplayName, amount))
            return DepositItemResult.DontHaveThatMany;

        var totalCost = amount * costPerItem;

        if (!source.TryTakeGold(totalCost))
            return DepositItemResult.NotEnoughGold;

        source.Inventory.RemoveQuantity(itemName, amount, out var items);

        foreach (var item in items!)
            source.Bank.Deposit(item);

        return DepositItemResult.Success;
    }

    public static SellItemResult SellItem(
        Aisling source,
        byte slot,
        int amount,
        int valuePerItem
    )
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

        var totalValue = valuePerItem * amount;

        if (!source.TryGiveGold(totalValue))
            return SellItemResult.TooMuchGold;

        source.Inventory.RemoveQuantity(slot, amount, out _);

        return SellItemResult.Success;
    }

    public static SellItemResult SellItem(
        Aisling source,
        string itemName,
        int amount,
        int valuePerItem
    )
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

        var totalValue = valuePerItem * amount;

        if (!source.TryGiveGold(totalValue))
            return SellItemResult.TooMuchGold;

        source.Inventory.RemoveQuantity(itemName, amount, out _);

        return SellItemResult.Success;
    }

    public static WithdrawGoldResult WithdrawGold(Aisling source, int amount)
    {
        if (amount < 0)
            return WithdrawGoldResult.BadInput;

        if (source.Bank.Gold < amount)
            return WithdrawGoldResult.NotEnoughGold;

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