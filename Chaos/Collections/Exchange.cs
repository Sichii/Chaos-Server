using Chaos.Common.Identity;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Observers;
using Microsoft.Extensions.Logging;

namespace Chaos.Collections;

public sealed class Exchange
{
    private readonly ILogger Logger;
    private readonly AutoReleasingMonitor Sync;
    private readonly Inventory User1Items;
    private readonly Inventory User2Items;
    private bool IsActive;
    private bool User1Accept;
    private int User1Gold;
    private bool User2Accept;
    private int User2Gold;
    public ulong ExchangeId { get; }
    public Aisling User1 { get; }
    public Aisling User2 { get; }

    public Exchange(Aisling sender, Aisling receiver, ILogger logger)
    {
        ExchangeId = PersistentIdGenerator<ulong>.Shared.NextId;
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

        IsActive = true;

        User1.Client.SendExchangeStart(User2);
        User2.Client.SendExchangeStart(User1);

        Logger.WithProperty(this)
              .LogDebug(
                  "Exchange {@ExchangeId} started between {@AislingName1} and {@AislingName2}",
                  ExchangeId,
                  User1.Name,
                  User2.Name);
    }

    public void AddItem(Aisling aisling, byte slot)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (_, var userItems, var userAccepted) = GetUserVars(aisling);

        if (!IsActive || !aisling.Inventory.TryGetObject(slot, out var item) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is account bound");

            return;
        }

        if (!otherUser.CanCarry(userItems.Prepend(item).ToArray()))
        {
            aisling.SendActiveMessage($"{otherUser.Name} is unable to carry that");
            otherUser.SendActiveMessage("You are unable to carry more");

            return;
        }

        if (item.Template.Stackable)
            aisling.Client.SendExchangeRequestAmount(item.Slot);
        else
        {
            aisling.Inventory.Remove(slot);
            userItems.TryAddToNextSlot(item);

            Logger.WithProperty(aisling)
                  .WithProperty(item)
                  .WithProperty(this)
                  .LogDebug(
                      "Aisling {@AislingName} added item {@ItemName} to exchange {@ExchangeId}",
                      aisling.Name,
                      item.DisplayName,
                      ExchangeId);
        }
    }

    public void AddStackableItem(Aisling aisling, byte slot, byte amount)
    {
        using var sync = Sync.Enter();

        var otherUser = GetOtherUser(aisling);
        (_, var userItems, var userAccepted) = GetUserVars(aisling);

        if (!IsActive || (amount <= 0) || !aisling.Inventory.TryGetObject(slot, out var item) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is account bound");

            return;
        }

        if (!aisling.Inventory.HasCount(item.DisplayName, amount))
        {
            aisling.SendActiveMessage($"You don't have {amount} of {item.DisplayName}");

            return;
        }

        var hypotheticalItems = userItems
                                .Select(i => (i, i.Count))
                                .Append((item, amount));

        if (!otherUser.CanCarry(hypotheticalItems))
        {
            aisling.SendActiveMessage($"{otherUser.Name} is unable to carry that");
            otherUser.SendActiveMessage("You are unable to carry more");

            return;
        }

        if (!aisling.Inventory.RemoveQuantity(item.Slot, amount, out var removedItems))
            return;

        foreach (var removedItem in removedItems)
        {
            userItems.TryAddToNextSlot(removedItem);

            Logger.WithProperty(aisling)
                  .WithProperty(removedItem)
                  .WithProperty(this)
                  .LogDebug(
                      "Aisling {@AislingName} added item {@ItemName} to exchange {@ExchangeId}",
                      aisling.Name,
                      removedItem.DisplayName,
                      ExchangeId);
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

        Logger.WithProperty(aisling)
              .WithProperty(this)
              .LogDebug("Exchange {@ExchangeId} was canceled by aisling {@AislingName}", ExchangeId, aisling.Name);

        Deactivate();
    }

    private void Deactivate()
    {
        IsActive = false;

        User1.ActiveObject.TryRemove(this);
        User2.ActiveObject.TryRemove(this);
    }

    private void Distribute(Aisling aisling, int gold, Inventory items)
    {
        aisling.TryGiveGold(gold);

        Logger.WithProperty(aisling)
              .WithProperty(this)
              .LogDebug(
                  "Exchange {@ExchangeId} distributed {Amount} gold to {@AislingName}",
                  ExchangeId,
                  gold,
                  aisling.Name);

        foreach (var item in items)
        {
            items.Remove(item.Slot);

            if (aisling.Inventory.TryAddToNextSlot(item))
                Logger.WithProperty(aisling)
                      .WithProperty(item)
                      .WithProperty(this)
                      .LogDebug(
                          "Exchange {@ExchangeId} distributed item {@ItemName} to {@AislingName}",
                          ExchangeId,
                          item.DisplayName,
                          aisling.Name);
            else
                Logger.WithProperty(aisling)
                      .WithProperty(item)
                      .WithProperty(this)
                      .LogCritical("Exchange {@ExchangeId} failed to distribute item {@ItemName}", ExchangeId, item.DisplayName);
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
        aisling.TryGiveGold(gold);
        SetUserGold(aisling, 0);

        if (aisling.TryTakeGold(amount))
        {
            SetUserGold(aisling, amount);

            Logger.WithProperty(aisling)
                  .WithProperty(this)
                  .LogDebug(
                      "Aisling {@AislingName} set their gold amount to {GoldAmount} for exchange {@ExchangeId}",
                      aisling.Name,
                      amount,
                      ExchangeId);
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