using System.Linq;
using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Objects.World;
using Chaos.Observers;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Exchange
{
    private readonly ILogger Logger;
    private readonly AutoReleasingMonitor Sync;
    private readonly User User1;
    private readonly Inventory User1Items;
    private readonly User User2;
    private readonly Inventory User2Items;
    private ulong ExchangeId;
    private bool IsActive;
    private bool User1Accept;
    private int User1Gold;
    private bool User2Accept;
    private int User2Gold;

    public Exchange(User sender, User receiver, ILogger<Exchange> logger)
    {
        ExchangeId = ServerId.NextId;
        Logger = logger;
        User1 = sender;
        User2 = receiver;
        User1Items = new Inventory();
        User1Items.AddObserver(new ExchangeObserver(User1, User2));
        User2Items = new Inventory();
        User2Items.AddObserver(new ExchangeObserver(User2, User1));
        Sync = new AutoReleasingMonitor();
    }

    public void Accept(User user)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(user);
        (var gold, var items, var accepted) = GetUserVars(user);
        (var otherGold, var otherItems, var otherAccepted) = GetUserVars(otherUser);

        if (!IsActive || accepted)
            return;

        SetUserAccepted(user, true);
        accepted = true;

        otherUser.Client.SendExchangeAccepted(true);

        if (accepted && otherAccepted)
        {
            Distribute(user, otherGold, otherItems);
            Distribute(otherUser, gold, items);

            user.Client.SendExchangeAccepted(false);
            otherUser.Client.SendExchangeAccepted(false);

            Deactivate();
        }
    }

    public void Activate()
    {
        using var sync = Sync.Enter();

        User1.UserState |= UserState.Exchanging;
        User2.UserState |= UserState.Exchanging;
        User1.Client.SendExchangeStart(User2);
        User2.Client.SendExchangeStart(User1);

        Logger.LogDebug(
            "Starting exchange between {User1Name} and {User2Name} with exchange id {ExchangeId}",
            User1.Name,
            User2.Name,
            ExchangeId);

        IsActive = true;
    }

    public void AddItem(User user, byte slot)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(user);
        (_, var userItems, var userAccepted) = GetUserVars(user);

        if (!IsActive || !user.Inventory.TryGetObject(slot, out var item) || (item == null) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{item.DisplayName} is account bound");

            return;
        }

        if (!otherUser.CanCarry(userItems.Prepend(item).ToArray()))
        {
            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} is unable to carry that");
            otherUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are unable to carry more");

            return;
        }

        if (item.Template.Stackable)
            user.Client.SendExchangeRequestAmount(item.Slot);
        else
        {
            user.Inventory.Remove(slot);
            userItems.TryAddToNextSlot(item);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} added {Item}",
                ExchangeId,
                user.Name,
                item);
        }
    }

    public void AddStackableItem(User user, byte slot, byte amount)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(user);
        (_, var userItems, var userAccepted) = GetUserVars(user);

        if (!IsActive || (amount <= 0) || !user.Inventory.TryGetObject(slot, out var item) || (item == null) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{item.DisplayName} is account bound");

            return;
        }

        if (!user.Inventory.HasCount(item.DisplayName, amount))
        {
            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You don't have {amount} of {item.DisplayName}");

            return;
        }

        //we need to check if they can carry the items before we actually take or give anything
        var cloned = ItemUtility.Instance.Clone(item);
        cloned.Count = amount;
        var corrected = cloned.FixStacks();

        if (!otherUser.CanCarry(userItems.Concat(corrected).ToArray()))
        {
            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} is unable to carry that");
            otherUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are unable to carry more");

            return;
        }

        if (!user.Inventory.RemoveQuantity(item.DisplayName, amount, out var removedItems))
            return;

        foreach (var removedItem in removedItems)
        {
            userItems.TryAddToNextSlot(removedItem);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} added {Item}",
                ExchangeId,
                user.Name,
                item);
        }
    }

    public void Cancel(User user)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(user);
        (var gold, var items, _) = GetUserVars(user);
        (var otherGold, var otherItems, _) = GetUserVars(otherUser);

        if (!IsActive)
            return;

        Distribute(user, gold, items);
        Distribute(otherUser, otherGold, otherItems);

        user.Client.SendExchangeCancel(false);
        otherUser.Client.SendExchangeCancel(true);
        Logger.LogDebug("[Exchange: {ExchangeId}]: {UserName} canceled the trade", ExchangeId, user.Name);

        Deactivate();
    }

    private void Deactivate()
    {
        IsActive = false;
        if(User1.ActiveObject.TryRemove(this))
            User1.UserState &= ~UserState.Exchanging;

        if (User2.ActiveObject.TryRemove(this))
            User2.UserState &= ~UserState.Exchanging;

    }

    private void Distribute(User user, int gold, Inventory items)
    {
        user.GiveGold(gold);

        Logger.LogDebug(
            "[Exchange: {ExchangeId}]: {UserName} received {Gold} gold",
            ExchangeId,
            user.Name,
            gold);

        foreach (var item in items)
        {
            items.Remove(item.Slot);

            if (user.Inventory.TryAddToNextSlot(item))
                Logger.LogDebug(
                    "[Exchange: {ExchangeId}]: {UserName} received {Item}",
                    ExchangeId,
                    user.Name,
                    item);
        }
    }

    public User GetOtherUser(User user) => user.Equals(User1) ? User2 : User1;

    private (int Gold, Inventory Items, bool Accepted) GetUserVars(User user) =>
        user.Equals(User1) ? (User1Gold, User1Items, User1Accept) : (User2Gold, User2Items, User2Accept);

    public void SetGold(User user, int amount)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(user);
        var uuserVars = GetUserVars(user);
        (var gold, _, var accepted) = uuserVars;

        if (!IsActive || accepted)
            return;

        //this is a set, so we should start by returning whatever gold is already in the exchange
        user.GiveGold(gold);
        SetUserGold(user, 0);

        if (user.Gold >= amount)
        {
            user.GiveGold(-amount);
            SetUserGold(user, amount);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} set his gold amount to {Gold}",
                ExchangeId,
                user.Name,
                gold);
        }

        user.Client.SendExchangeSetGold(false, amount);
        otherUser.Client.SendExchangeSetGold(true, amount);
    }

    private void SetUserAccepted(User user, bool accepted)
    {
        if (user.Equals(User1))
            User1Accept = accepted;
        else
            User2Accept = accepted;
    }

    private void SetUserGold(User user, int amount)
    {
        if (user.Equals(User1))
            User1Gold = amount;
        else
            User2Gold = amount;
    }
}