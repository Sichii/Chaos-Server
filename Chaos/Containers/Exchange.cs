using Chaos.Core.Identity;
using Chaos.Core.Synchronization;
using Chaos.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Objects.World;
using Chaos.Observers;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Exchange
{
    private readonly ulong ExchangeId;
    private readonly ILogger Logger;
    private readonly AutoReleasingMonitor Sync;
    private readonly Aisling User1;
    private readonly Inventory User1Items;
    private readonly Aisling User2;
    private readonly Inventory User2Items;
    private bool IsActive;
    private bool User1Accept;
    private int User1Gold;
    private bool User2Accept;
    private int User2Gold;

    public Exchange(Aisling sender, Aisling receiver, ILogger logger)
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

    public void Accept(Aisling aisling)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (var gold, var items, var accepted) = GetUserVars(aisling);
        (var otherGold, var otherItems, var otherAccepted) = GetUserVars(otherUser);

        if (!IsActive || accepted)
            return;

        SetUserAccepted(aisling, true);
        accepted = true;

        otherUser.Client.SendExchangeAccepted(true);

        if (accepted && otherAccepted)
        {
            Distribute(aisling, otherGold, otherItems);
            Distribute(otherUser, gold, items);

            aisling.Client.SendExchangeAccepted(false);
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

    public void AddItem(Aisling aisling, byte slot)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (_, var userItems, var userAccepted) = GetUserVars(aisling);

        if (!IsActive || !aisling.Inventory.TryGetObject(slot, out var item) || (item == null) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{item.DisplayName} is account bound");

            return;
        }

        if (!otherUser.CanCarry(userItems.Prepend(item).ToArray()))
        {
            aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} is unable to carry that");
            otherUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are unable to carry more");

            return;
        }

        if (item.Template.Stackable)
            aisling.Client.SendExchangeRequestAmount(item.Slot);
        else
        {
            aisling.Inventory.Remove(slot);
            userItems.TryAddToNextSlot(item);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} added {Item}",
                ExchangeId,
                aisling.Name,
                item);
        }
    }

    public void AddStackableItem(Aisling aisling, byte slot, byte amount)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (_, var userItems, var userAccepted) = GetUserVars(aisling);

        if (!IsActive || (amount <= 0) || !aisling.Inventory.TryGetObject(slot, out var item) || (item == null) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{item.DisplayName} is account bound");

            return;
        }

        if (!aisling.Inventory.HasCount(item.DisplayName, amount))
        {
            aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You don't have {amount} of {item.DisplayName}");

            return;
        }

        var hypotheticalItems = userItems
                                .Select(i => (i, i.Count))
                                .Append((item, amount));

        if (!otherUser.CanCarry(hypotheticalItems))
        {
            aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} is unable to carry that");
            otherUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are unable to carry more");

            return;
        }

        if (!aisling.Inventory.RemoveQuantity(item.DisplayName, amount, out var removedItems))
            return;

        foreach (var removedItem in removedItems)
        {
            userItems.TryAddToNextSlot(removedItem);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} added {Item}",
                ExchangeId,
                aisling.Name,
                item);
        }
    }

    public void Cancel(Aisling aisling)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (var gold, var items, _) = GetUserVars(aisling);
        (var otherGold, var otherItems, _) = GetUserVars(otherUser);

        if (!IsActive)
            return;

        Distribute(aisling, gold, items);
        Distribute(otherUser, otherGold, otherItems);

        aisling.Client.SendExchangeCancel(false);
        otherUser.Client.SendExchangeCancel(true);
        Logger.LogDebug("[Exchange: {ExchangeId}]: {UserName} canceled the trade", ExchangeId, aisling.Name);

        Deactivate();
    }

    private void Deactivate()
    {
        IsActive = false;

        if (User1.ActiveObject.TryRemove(this))
            User1.UserState &= ~UserState.Exchanging;

        if (User2.ActiveObject.TryRemove(this))
            User2.UserState &= ~UserState.Exchanging;
    }

    private void Distribute(Aisling aisling, int gold, Inventory items)
    {
        aisling.GiveGold(gold);

        Logger.LogDebug(
            "[Exchange: {ExchangeId}]: {UserName} received {Gold} gold",
            ExchangeId,
            aisling.Name,
            gold);

        foreach (var item in items)
        {
            items.Remove(item.Slot);

            if (aisling.Inventory.TryAddToNextSlot(item))
                Logger.LogDebug(
                    "[Exchange: {ExchangeId}]: {UserName} received {Item}",
                    ExchangeId,
                    aisling.Name,
                    item);
        }
    }

    public Aisling GetOtherUser(Aisling aisling) => aisling.Equals(User1) ? User2 : User1;

    private (int Gold, Inventory Items, bool Accepted) GetUserVars(Aisling aisling) =>
        aisling.Equals(User1) ? (User1Gold, User1Items, User1Accept) : (User2Gold, User2Items, User2Accept);

    public void SetGold(Aisling aisling, int amount)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        var uuserVars = GetUserVars(aisling);
        (var gold, _, var accepted) = uuserVars;

        if (!IsActive || accepted)
            return;

        //this is a set, so we should start by returning whatever gold is already in the exchange
        aisling.GiveGold(gold);
        SetUserGold(aisling, 0);

        if (aisling.Gold >= amount)
        {
            aisling.GiveGold(-amount);
            SetUserGold(aisling, amount);

            Logger.LogDebug(
                "[Exchange: {ExchangeId}]: {UserName} set his gold amount to {Gold}",
                ExchangeId,
                aisling.Name,
                gold);
        }

        aisling.Client.SendExchangeSetGold(false, amount);
        otherUser.Client.SendExchangeSetGold(true, amount);
    }

    private void SetUserAccepted(Aisling aisling, bool accepted)
    {
        if (aisling.Equals(User1))
            User1Accept = accepted;
        else
            User2Accept = accepted;
    }

    private void SetUserGold(Aisling aisling, int amount)
    {
        if (aisling.Equals(User1))
            User1Gold = amount;
        else
            User2Gold = amount;
    }
}