#region
using Chaos.Common.Identity;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Observers;
using Humanizer;
#endregion

namespace Chaos.Collections;

public sealed class Exchange
{
    private readonly Aisling Aisling1;
    private readonly Inventory Aisling1Items;
    private readonly Aisling Aisling2;
    private readonly Inventory Aisling2Items;
    private readonly ILogger<Exchange> Logger;
    private readonly Lock Sync;
    private bool Aisling1Accept;
    private int Aisling1Gold;
    private bool Aisling2Accept;
    private int Aisling2Gold;
    private bool IsActive;
    public ulong ExchangeId { get; }

    public Exchange(Aisling sender, Aisling receiver, ILogger<Exchange> logger)
    {
        ExchangeId = PersistentIdGenerator<ulong>.Shared.NextId;
        Logger = logger;
        Aisling1 = sender;
        Aisling2 = receiver;
        Aisling1Items = new Inventory();
        Aisling1Items.AddObserver(new ExchangeObserver(Aisling1, Aisling2));
        Aisling2Items = new Inventory();
        Aisling2Items.AddObserver(new ExchangeObserver(Aisling2, Aisling1));
        Sync = new Lock();
    }

    public void Accept(Aisling aisling)
    {
        using var sync = Sync.EnterScope();

        var otherUser = GetOther(aisling);
        (var gold, var items, var accepted) = InnerGetVars(aisling);
        (var otherGold, var otherItems, var otherAccepted) = InnerGetVars(otherUser);

        if (!IsActive || accepted)
            return;

        InnerSetAccepted(aisling, true);
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
        using var sync = Sync.EnterScope();

        IsActive = true;

        Aisling1.Client.SendExchangeStart(Aisling2);
        Aisling2.Client.SendExchangeStart(Aisling1);

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Exchange,
                      Topics.Actions.Create
                  ])
              .WithProperty(this)
              .LogDebug(
                  "Exchange {@ExchangeId} started between {@AislingName1} and {@AislingName2}",
                  ExchangeId,
                  Aisling1.Name,
                  Aisling2.Name);
    }

    public void AddItem(Aisling aisling, byte slot)
    {
        using var sync = Sync.EnterScope();

        var otherUser = GetOther(aisling);
        (_, var userItems, var userAccepted) = InnerGetVars(aisling);

        if (!IsActive || !aisling.Inventory.TryGetObject(slot, out var item) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is account bound");

            return;
        }

        if (item.Template.NoTrade)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is no-trade");

            return;
        }

        if (item.Template.Stackable)
            aisling.Client.SendExchangeRequestAmount(item.Slot);
        else
        {
            if (!otherUser.CanCarry(
                    userItems.Prepend(item)
                             .ToArray()))
            {
                aisling.SendActiveMessage($"{otherUser.Name} is unable to carry that");
                otherUser.SendActiveMessage("You are unable to carry more");

                return;
            }

            aisling.Inventory.Remove(slot);
            userItems.TryAddToNextSlot(item);

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Exchange,
                          Topics.Entities.Item,
                          Topics.Actions.Add
                      ])
                  .WithProperty(aisling)
                  .WithProperty(item)
                  .WithProperty(this)
                  .LogInformation(
                      "Aisling {@AislingName} added item {@ItemName} to exchange {@ExchangeId}",
                      aisling.Name,
                      item.DisplayName,
                      ExchangeId);
        }
    }

    public void AddStackableItem(Aisling aisling, byte slot, byte amount)
    {
        using var sync = Sync.EnterScope();

        var otherUser = GetOther(aisling);
        (_, var userItems, var userAccepted) = InnerGetVars(aisling);

        if (!IsActive || (amount <= 0) || !aisling.Inventory.TryGetObject(slot, out var item) || userAccepted)
            return;

        if (item.Template.AccountBound)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is account bound");

            return;
        }

        if (item.Template.NoTrade)
        {
            aisling.SendActiveMessage($"{item.DisplayName} is no-trade");

            return;
        }

        if (!aisling.Inventory.HasCount(item.DisplayName, amount))
        {
            aisling.SendActiveMessage($"You don't have {item.DisplayName.ToQuantity(amount)}");

            return;
        }

        var hypotheticalItems = userItems.Select(i => (i, i.Count))
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

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Exchange,
                          Topics.Entities.Item,
                          Topics.Actions.Add
                      ])
                  .WithProperty(aisling)
                  .WithProperty(removedItem)
                  .WithProperty(this)
                  .LogInformation(
                      "Aisling {@AislingName} added item {@ItemName} to exchange {@ExchangeId}",
                      aisling.Name,
                      removedItem.DisplayName,
                      ExchangeId);
        }
    }

    public void Cancel(Aisling aisling)
    {
        using var sync = Sync.EnterScope();

        var otherUser = GetOther(aisling);
        (var gold, var items, _) = InnerGetVars(aisling);
        (var otherGold, var otherItems, _) = InnerGetVars(otherUser);

        if (!IsActive)
            return;

        Distribute(aisling, gold, items);
        Distribute(otherUser, otherGold, otherItems);

        aisling.Client.SendExchangeCancel(false);
        otherUser.Client.SendExchangeCancel(true);

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Exchange,
                      Topics.Actions.Canceled
                  ])
              .WithProperty(aisling)
              .WithProperty(this)
              .LogDebug("Exchange {@ExchangeId} was canceled by aisling {@AislingName}", ExchangeId, aisling.Name);

        Deactivate();
    }

    private void Deactivate()
    {
        IsActive = false;

        Aisling1.ActiveObject.TryRemove(this);
        Aisling2.ActiveObject.TryRemove(this);
    }

    private void Distribute(Aisling aisling, int gold, Inventory items)
    {
        aisling.TryGiveGold(gold);

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Exchange,
                      Topics.Entities.Gold,
                      Topics.Actions.Accepted
                  ])
              .WithProperty(aisling)
              .WithProperty(this)
              .LogInformation(
                  "Exchange {@ExchangeId} distributed {Amount} gold to {@AislingName}",
                  ExchangeId,
                  gold,
                  aisling.Name);

        foreach (var item in items)
        {
            items.Remove(item.Slot);

            if (aisling.Inventory.TryAddToNextSlot(item))
                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Exchange,
                              Topics.Entities.Item,
                              Topics.Actions.Accepted
                          ])
                      .WithProperty(aisling)
                      .WithProperty(item)
                      .WithProperty(this)
                      .LogInformation(
                          "Exchange {@ExchangeId} distributed item {@ItemName} to {@AislingName}",
                          ExchangeId,
                          item.DisplayName,
                          aisling.Name);
            else
                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Exchange,
                              Topics.Entities.Item,
                              Topics.Actions.Accepted
                          ])
                      .WithProperty(aisling)
                      .WithProperty(item)
                      .WithProperty(this)
                      .LogError("Exchange {@ExchangeId} failed to distribute item {@ItemName}", ExchangeId, item.DisplayName);
        }
    }

    public Aisling GetOther(Aisling aisling) => aisling.Equals(Aisling1) ? Aisling2 : Aisling1;

    private (int Gold, Inventory Items, bool Accepted) InnerGetVars(Aisling aisling)
        => aisling.Equals(Aisling1) ? (Aisling1Gold, Aisling1Items, Aisling1Accept) : (Aisling2Gold, Aisling2Items, Aisling2Accept);

    private void InnerSetAccepted(Aisling aisling, bool accepted)
    {
        if (aisling.Equals(Aisling1))
            Aisling1Accept = accepted;
        else
            Aisling2Accept = accepted;
    }

    private void InnerSetGold(Aisling aisling, int amount)
    {
        if (aisling.Equals(Aisling1))
            Aisling1Gold = amount;
        else
            Aisling2Gold = amount;
    }

    public void SetGold(Aisling aisling, int amount)
    {
        using var sync = Sync.EnterScope();

        var otherUser = GetOther(aisling);
        var uuserVars = InnerGetVars(aisling);
        (var gold, _, var accepted) = uuserVars;

        if (!IsActive || accepted)
            return;

        //this is a set, so we should start by returning whatever gold is already in the exchange
        aisling.TryGiveGold(gold);
        InnerSetGold(aisling, 0);

        if (aisling.TryTakeGold(amount))
        {
            InnerSetGold(aisling, amount);

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Exchange,
                          Topics.Entities.Gold,
                          Topics.Actions.Update
                      ])
                  .WithProperty(aisling)
                  .WithProperty(this)
                  .LogInformation(
                      "Aisling {@AislingName} set their gold amount to {GoldAmount} for exchange {@ExchangeId}",
                      aisling.Name,
                      amount,
                      ExchangeId);
        }

        aisling.Client.SendExchangeSetGold(false, amount);
        otherUser.Client.SendExchangeSetGold(true, amount);
    }
}