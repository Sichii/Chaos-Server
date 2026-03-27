#region
using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class ExchangeTests
{
    #region Distribute Edge Cases
    [Test]
    public void Accept_BothAccept_ShouldHandleFullInventory()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        // Fill receiver inventory to capacity (slots 1-59)
        for (var i = 0; i < 59; i++)
        {
            var filler = MockItem.Create($"Filler{i}");
            receiver.Inventory.TryAddToNextSlot(filler);
        }

        receiver.Inventory
                .IsFull
                .Should()
                .BeTrue();

        var senderItem = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(senderItem);
        exchange.AddItem(sender, senderItem.Slot);

        exchange.Accept(sender);
        exchange.Accept(receiver);

        // Item can't be added to full inventory — exchange still completes (logs error)
        receiver.Inventory
                .Contains("Sword")
                .Should()
                .BeFalse();
    }
    #endregion

    #region Activate
    [Test]
    public void Activate_ShouldSendExchangeStartToBothClients()
    {
        (var sender, var receiver, var exchange, var senderClient, var receiverClient) = CreateExchange();

        exchange.Activate();

        senderClient.Verify(c => c.SendExchangeStart(receiver), Times.Once);
        receiverClient.Verify(c => c.SendExchangeStart(sender), Times.Once);
    }
    #endregion

    private static (Aisling Sender, Aisling Receiver, Exchange Exchange, Mock<IChaosWorldClient> SenderClient, Mock<IChaosWorldClient>
        ReceiverClient) CreateExchange()
    {
        var map = MockMapInstance.Create();

        var sender = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(100));
        var receiver = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(100));

        var senderClient = Mock.Get(sender.Client);
        var receiverClient = Mock.Get(receiver.Client);

        var loggerMock = new Mock<ILogger<Exchange>>();
        var exchange = new Exchange(sender, receiver, loggerMock.Object);

        return (sender, receiver, exchange, senderClient, receiverClient);
    }

    #region GetOther
    [Test]
    public void GetOther_ShouldReturnReceiver_WhenGivenSender()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();

        exchange.GetOther(sender)
                .Should()
                .BeSameAs(receiver);
    }

    [Test]
    public void GetOther_ShouldReturnSender_WhenGivenReceiver()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();

        exchange.GetOther(receiver)
                .Should()
                .BeSameAs(sender);
    }
    #endregion

    #region AddItem
    [Test]
    public void AddItem_ShouldDoNothing_WhenExchangeNotActive()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();

        var item = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(item);

        // Not activated yet
        exchange.AddItem(sender, item.Slot);

        // Item should still be in inventory
        sender.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddItem_ShouldRemoveItemFromInventory_WhenActive()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(sender, item.Slot);

        sender.Inventory
              .Contains("Sword")
              .Should()
              .BeFalse();
    }

    [Test]
    public void AddItem_ShouldBlockAccountBoundItems()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("BoundSword", setup: i => i.AccountBound = true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(sender, item.Slot);

        // Item should still be in inventory (blocked)
        sender.Inventory
              .Contains("BoundSword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddItem_ShouldBlockNoTradeItems()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("NoTradeSword", setup: i => i.NoTrade = true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(sender, item.Slot);

        // Item should still be in inventory (blocked)
        sender.Inventory
              .Contains("NoTradeSword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddItem_ShouldRequestAmount_WhenItemIsStackable()
    {
        (var sender, _, var exchange, var senderClient, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Potion", 5, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(sender, item.Slot);

        senderClient.Verify(c => c.SendExchangeRequestAmount(item.Slot), Times.Once);
    }

    [Test]
    public void AddItem_ShouldDoNothing_WhenSlotIsInvalid()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        // Slot 99 has no item
        exchange.AddItem(sender, 99);

        // No crash, no side effects
        sender.Inventory
              .Should()
              .BeEmpty();
    }

    [Test]
    public void AddItem_ShouldBlock_WhenReceiverCannotCarry()
    {
        var map = MockMapInstance.Create();
        var sender = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(100));
        var receiver = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(0)); // zero weight capacity

        var loggerMock = new Mock<ILogger<Exchange>>();
        var exchange = new Exchange(sender, receiver, loggerMock.Object);
        exchange.Activate();

        var item = MockItem.Create("HeavySword", setup: i => i.Weight = 50);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(sender, item.Slot);

        // Item should still be in sender inventory (blocked by weight)
        sender.Inventory
              .Contains("HeavySword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddItem_ShouldDoNothing_WhenUserAlreadyAccepted()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(item);

        exchange.Accept(sender);

        // After accepting, should not be able to add items
        exchange.AddItem(sender, item.Slot);

        // Item should still be in inventory
        sender.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();
    }
    #endregion

    #region AddStackableItem
    [Test]
    public void AddStackableItem_ShouldDoNothing_WhenNotActive()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();

        var item = MockItem.Create("Potion", 5, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 3);

        sender.Inventory
              .HasCount("Potion", 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldDoNothing_WhenAmountIsZero()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Potion", 5, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 0);

        sender.Inventory
              .HasCount("Potion", 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldDoNothing_WhenSlotIsInvalid()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        exchange.AddStackableItem(sender, 99, 3);
    }

    [Test]
    public void AddStackableItem_ShouldDoNothing_WhenUserAlreadyAccepted()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Potion", 5, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.Accept(sender);
        exchange.AddStackableItem(sender, item.Slot, 3);

        sender.Inventory
              .HasCount("Potion", 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldBlockAccountBoundItems()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create(
            "BoundPotion",
            5,
            true,
            setup: i => i.AccountBound = true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 3);

        sender.Inventory
              .HasCount("BoundPotion", 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldBlockNoTradeItems()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create(
            "NoTradePotion",
            5,
            true,
            setup: i => i.NoTrade = true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 3);

        sender.Inventory
              .HasCount("NoTradePotion", 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldBlock_WhenInsufficientQuantity()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Potion", 3, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 10);

        // Should still have original amount
        sender.Inventory
              .HasCount("Potion", 3)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldBlock_WhenReceiverCannotCarry()
    {
        var map = MockMapInstance.Create();
        var sender = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(100));
        var receiver = MockAisling.Create(map, setup: a => a.UserStatSheet.SetMaxWeight(0));

        var loggerMock = new Mock<ILogger<Exchange>>();
        var exchange = new Exchange(sender, receiver, loggerMock.Object);
        exchange.Activate();

        var item = MockItem.Create(
            "HeavyPotion",
            10,
            true,
            setup: i => i.Weight = 50);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 5);

        sender.Inventory
              .HasCount("HeavyPotion", 10)
              .Should()
              .BeTrue();
    }

    [Test]
    public void AddStackableItem_ShouldRemoveFromInventory_WhenSuccessful()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        // Add entire stack to avoid Item.Split (which needs ItemCloner not available in test Aisling)
        var item = MockItem.Create("Potion", 5, true);
        sender.Inventory.TryAddToNextSlot(item);

        exchange.AddStackableItem(sender, item.Slot, 5);

        sender.Inventory
              .Contains("Potion")
              .Should()
              .BeFalse();
    }
    #endregion

    #region SetGold
    [Test]
    public void SetGold_ShouldDoNothing_WhenNotActive()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        sender.TryGiveGold(1000);

        exchange.SetGold(sender, 500);

        // Gold should not be taken
        sender.Gold
              .Should()
              .Be(1000);
    }

    [Test]
    public void SetGold_ShouldTakeGoldFromAisling()
    {
        (var sender, _, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();
        sender.TryGiveGold(1000);

        exchange.SetGold(sender, 500);

        sender.Gold
              .Should()
              .Be(500);

        senderClient.Verify(c => c.SendExchangeSetGold(false, 500), Times.Once);
        receiverClient.Verify(c => c.SendExchangeSetGold(true, 500), Times.Once);
    }

    [Test]
    public void SetGold_ShouldReturnPreviousGoldFirst()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();
        sender.TryGiveGold(1000);

        exchange.SetGold(sender, 300);

        sender.Gold
              .Should()
              .Be(700);

        // Set again with different amount; previous gold (300) returned first
        exchange.SetGold(sender, 500);

        sender.Gold
              .Should()
              .Be(500);
    }

    [Test]
    public void SetGold_ShouldSendZero_WhenInsufficientGold()
    {
        (var sender, _, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();
        sender.TryGiveGold(100);

        exchange.SetGold(sender, 500);

        // Can't afford 500, so gold amount in exchange is 0
        senderClient.Verify(c => c.SendExchangeSetGold(false, 0), Times.Once);
        receiverClient.Verify(c => c.SendExchangeSetGold(true, 0), Times.Once);
    }

    [Test]
    public void SetGold_ShouldDoNothing_WhenAlreadyAccepted()
    {
        (var sender, _, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();
        sender.TryGiveGold(1000);

        exchange.Accept(sender);

        senderClient.Invocations.Clear();
        receiverClient.Invocations.Clear();

        exchange.SetGold(sender, 500);

        // Gold should not change since already accepted
        sender.Gold
              .Should()
              .Be(1000);
        senderClient.Verify(c => c.SendExchangeSetGold(It.IsAny<bool>(), It.IsAny<int>()), Times.Never);
    }
    #endregion

    #region Accept
    [Test]
    public void Accept_ShouldDoNothing_WhenNotActive()
    {
        (var sender, _, var exchange, var senderClient, _) = CreateExchange();

        exchange.Accept(sender);

        senderClient.Verify(c => c.SendExchangeAccepted(It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void Accept_SingleAccept_ShouldNotDistribute()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        sender.TryGiveGold(500);
        exchange.SetGold(sender, 200);

        exchange.Accept(sender);

        // Gold should still be taken from sender (in exchange), not distributed yet
        receiver.Gold
                .Should()
                .Be(0);
    }

    [Test]
    public void Accept_BothAccept_ShouldDistributeGold()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        sender.TryGiveGold(1000);
        receiver.TryGiveGold(500);

        exchange.SetGold(sender, 300);
        exchange.SetGold(receiver, 100);

        exchange.Accept(sender);
        exchange.Accept(receiver);

        // sender gets receiver's gold (100), receiver gets sender's gold (300)
        sender.Gold
              .Should()
              .Be(800); // 1000 - 300 + 100

        receiver.Gold
                .Should()
                .Be(700); // 500 - 100 + 300
    }

    [Test]
    public void Accept_BothAccept_ShouldDistributeItems()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var senderItem = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(senderItem);
        exchange.AddItem(sender, senderItem.Slot);

        exchange.Accept(sender);
        exchange.Accept(receiver);

        // Receiver should now have the sword
        receiver.Inventory
                .Contains("Sword")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Accept_ShouldDoNothing_WhenAlreadyAccepted()
    {
        (var sender, _, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();

        exchange.Accept(sender);

        senderClient.Invocations.Clear();
        receiverClient.Invocations.Clear();

        // Accept again — should be ignored
        exchange.Accept(sender);

        // No additional exchange accepted messages should be sent for second accept
        receiverClient.Verify(c => c.SendExchangeAccepted(true), Times.Never);
    }
    #endregion

    #region Cancel
    [Test]
    public void Cancel_ShouldDoNothing_WhenNotActive()
    {
        (var sender, _, var exchange, var senderClient, _) = CreateExchange();

        exchange.Cancel(sender);

        senderClient.Verify(c => c.SendExchangeCancel(It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void Cancel_ShouldReturnGoldToOwners()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        sender.TryGiveGold(1000);
        receiver.TryGiveGold(500);

        exchange.SetGold(sender, 300);
        exchange.SetGold(receiver, 100);

        exchange.Cancel(sender);

        sender.Gold
              .Should()
              .Be(1000);

        receiver.Gold
                .Should()
                .Be(500);
    }

    [Test]
    public void Cancel_ShouldReturnItemsToOwners()
    {
        (var sender, _, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(item);
        exchange.AddItem(sender, item.Slot);

        // Item removed from sender inventory
        sender.Inventory
              .Contains("Sword")
              .Should()
              .BeFalse();

        exchange.Cancel(sender);

        // Item should be returned
        sender.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void Cancel_ShouldSendCancelToBothClients()
    {
        (var sender, _, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();

        exchange.Cancel(sender);

        senderClient.Verify(c => c.SendExchangeCancel(false), Times.Once);
        receiverClient.Verify(c => c.SendExchangeCancel(true), Times.Once);
    }

    [Test]
    public void Cancel_ShouldDeactivateExchange()
    {
        (var sender, _, var exchange, var senderClient, _) = CreateExchange();
        exchange.Activate();

        exchange.Cancel(sender);

        senderClient.Invocations.Clear();

        // After cancel, further operations should be no-ops
        exchange.Accept(sender);
        senderClient.Verify(c => c.SendExchangeAccepted(It.IsAny<bool>()), Times.Never);
    }
    #endregion

    #region Receiver (Aisling2) Operations
    [Test]
    public void SetGold_ShouldWorkForReceiver()
    {
        (_, var receiver, var exchange, _, var receiverClient) = CreateExchange();
        exchange.Activate();
        receiver.TryGiveGold(1000);

        exchange.SetGold(receiver, 400);

        receiver.Gold
                .Should()
                .Be(600);
        receiverClient.Verify(c => c.SendExchangeSetGold(false, 400), Times.Once);
    }

    [Test]
    public void AddItem_ShouldWorkForReceiver()
    {
        (_, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var item = MockItem.Create("Shield");
        receiver.Inventory.TryAddToNextSlot(item);

        exchange.AddItem(receiver, item.Slot);

        receiver.Inventory
                .Contains("Shield")
                .Should()
                .BeFalse();
    }

    [Test]
    public void Accept_ShouldWorkForReceiver()
    {
        (_, var receiver, var exchange, var senderClient, _) = CreateExchange();
        exchange.Activate();

        exchange.Accept(receiver);

        senderClient.Verify(c => c.SendExchangeAccepted(true), Times.Once);
    }
    #endregion

    #region Full Exchange Flow
    [Test]
    public void Accept_BothAccept_ShouldSendAcceptedFalseToBoth()
    {
        (var sender, var receiver, var exchange, var senderClient, var receiverClient) = CreateExchange();
        exchange.Activate();

        exchange.Accept(sender);
        exchange.Accept(receiver);

        senderClient.Verify(c => c.SendExchangeAccepted(false), Times.Once);
        receiverClient.Verify(c => c.SendExchangeAccepted(false), Times.Once);
    }

    [Test]
    public void Accept_BothAccept_ShouldDistributeItemsFromBothSides()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        var senderItem = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(senderItem);
        exchange.AddItem(sender, senderItem.Slot);

        var receiverItem = MockItem.Create("Shield");
        receiver.Inventory.TryAddToNextSlot(receiverItem);
        exchange.AddItem(receiver, receiverItem.Slot);

        exchange.Accept(sender);
        exchange.Accept(receiver);

        // Items should be swapped
        receiver.Inventory
                .Contains("Sword")
                .Should()
                .BeTrue();

        sender.Inventory
              .Contains("Shield")
              .Should()
              .BeTrue();
    }

    [Test]
    public void Accept_BothAccept_ShouldDeactivateExchange()
    {
        (var sender, var receiver, var exchange, var senderClient, _) = CreateExchange();
        exchange.Activate();

        exchange.Accept(sender);
        exchange.Accept(receiver);

        senderClient.Invocations.Clear();

        // After completion, exchange should be deactivated
        exchange.SetGold(sender, 100);
        senderClient.Verify(c => c.SendExchangeSetGold(It.IsAny<bool>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void Accept_BothAccept_ShouldDistributeGoldAndItems()
    {
        (var sender, var receiver, var exchange, _, _) = CreateExchange();
        exchange.Activate();

        sender.TryGiveGold(1000);
        receiver.TryGiveGold(500);

        exchange.SetGold(sender, 200);
        exchange.SetGold(receiver, 50);

        var senderItem = MockItem.Create("Sword");
        sender.Inventory.TryAddToNextSlot(senderItem);
        exchange.AddItem(sender, senderItem.Slot);

        exchange.Accept(sender);
        exchange.Accept(receiver);

        // Gold distributed: sender gets 50, receiver gets 200
        sender.Gold
              .Should()
              .Be(850); // 1000 - 200 + 50

        receiver.Gold
                .Should()
                .Be(650); // 500 - 50 + 200

        // Item distributed
        receiver.Inventory
                .Contains("Sword")
                .Should()
                .BeTrue();
    }
    #endregion
}