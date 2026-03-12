#region
using Chaos.Models.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class ComplexActionHelperTests
{
    #region WithdrawGold - additional
    [Test]
    public void WithdrawGold_ShouldReturnTooMuchGold_WhenGoldWouldOverflow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(9_999_999);
        aisling.Bank.AddGold(500);

        ComplexActionHelper.WithdrawGold(aisling, 500)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawGoldResult.TooMuchGold);
    }
    #endregion

    #region DepositGold
    [Test]
    public void DepositGold_ShouldSucceed_WhenAislingHasEnoughGold()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(500);

        var result = ComplexActionHelper.DepositGold(aisling, 200);

        result.Should()
              .Be(ComplexActionHelper.DepositGoldResult.Success);

        aisling.Gold
               .Should()
               .Be(300);

        aisling.Bank
               .Gold
               .Should()
               .Be(200);
    }

    [Test]
    public void DepositGold_ShouldReturnBadInput_WhenAmountIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(500);

        ComplexActionHelper.DepositGold(aisling, -1)
                           .Should()
                           .Be(ComplexActionHelper.DepositGoldResult.BadInput);
    }

    [Test]
    public void DepositGold_ShouldReturnDontHaveThatMany_WhenInsufficientGold()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(100);

        ComplexActionHelper.DepositGold(aisling, 500)
                           .Should()
                           .Be(ComplexActionHelper.DepositGoldResult.DontHaveThatMany);
    }
    #endregion

    #region WithdrawGold
    [Test]
    public void WithdrawGold_ShouldSucceed_WhenBankHasEnoughGold()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.Bank.AddGold(500);

        var result = ComplexActionHelper.WithdrawGold(aisling, 200);

        result.Should()
              .Be(ComplexActionHelper.WithdrawGoldResult.Success);

        aisling.Gold
               .Should()
               .Be(200);

        aisling.Bank
               .Gold
               .Should()
               .Be(300);
    }

    [Test]
    public void WithdrawGold_ShouldReturnBadInput_WhenAmountIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.WithdrawGold(aisling, -1)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawGoldResult.BadInput);
    }

    [Test]
    public void WithdrawGold_ShouldReturnDontHaveThatMany_WhenBankHasInsufficientGold()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.Bank.AddGold(100);

        ComplexActionHelper.WithdrawGold(aisling, 500)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawGoldResult.DontHaveThatMany);
    }
    #endregion

    #region DepositItem (by slot)
    [Test]
    public void DepositItem_BySlot_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        var result = ComplexActionHelper.DepositItem(aisling, item.Slot, 1);

        result.Should()
              .Be(ComplexActionHelper.DepositItemResult.Success);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeFalse();

        aisling.Bank
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(aisling, 1, 0)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnBadInput_WhenCostIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(
                               aisling,
                               1,
                               1,
                               -1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnBadInput_WhenSlotIsEmpty()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(aisling, 1, 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnDontHaveThatMany_WhenInsufficientQuantity()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Potion", 3, true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(aisling, item.Slot, 10)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.DontHaveThatMany);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnNotEnoughGold_WhenCantAffordCost()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(
                               aisling,
                               item.Slot,
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.NotEnoughGold);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnBadInput_WhenItemPreventsbanking()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("BoundItem", setup: i => i.PreventBanking = true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(aisling, item.Slot, 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }
    #endregion

    #region DepositItem (by name)
    [Test]
    public void DepositItem_ByName_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        var result = ComplexActionHelper.DepositItem(aisling, "Sword", 1);

        result.Should()
              .Be(ComplexActionHelper.DepositItemResult.Success);

        aisling.Bank
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnBadInput_WhenItemNotFound()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(aisling, "Nonexistent", 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }
    #endregion

    #region SellItem (by slot)
    [Test]
    public void SellItem_BySlot_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        var result = ComplexActionHelper.SellItem(
            aisling,
            item.Slot,
            1,
            50);

        result.Should()
              .Be(ComplexActionHelper.SellItemResult.Success);

        aisling.Gold
               .Should()
               .Be(50);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeFalse();
    }

    [Test]
    public void SellItem_BySlot_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               1,
                               0,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }

    [Test]
    public void SellItem_BySlot_ShouldReturnBadInput_WhenValueIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               1,
                               1,
                               -1)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }

    [Test]
    public void SellItem_BySlot_ShouldReturnBadInput_WhenSlotIsEmpty()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               1,
                               1,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }

    [Test]
    public void SellItem_BySlot_ShouldReturnDontHaveThatMany_WhenInsufficientQuantity()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Potion", 3, true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.SellItem(
                               aisling,
                               item.Slot,
                               10,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.DontHaveThatMany);
    }
    #endregion

    #region SellItem (by name)
    [Test]
    public void SellItem_ByName_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        var result = ComplexActionHelper.SellItem(
            aisling,
            "Sword",
            1,
            50);

        result.Should()
              .Be(ComplexActionHelper.SellItemResult.Success);

        aisling.Gold
               .Should()
               .Be(50);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnBadInput_WhenItemNotFound()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               "Nonexistent",
                               1,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               "Sword",
                               0,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnBadInput_WhenValueIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.SellItem(
                               aisling,
                               "Sword",
                               1,
                               -1)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.BadInput);
    }
    #endregion

    #region WithdrawItem
    [Test]
    public void WithdrawItem_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Bank.Deposit(item);

        var result = ComplexActionHelper.WithdrawItem(aisling, "Sword", 1);

        result.Should()
              .Be(ComplexActionHelper.WithdrawItemResult.Success);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();

        aisling.Bank
               .Contains("Sword")
               .Should()
               .BeFalse();
    }

    [Test]
    public void WithdrawItem_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.WithdrawItem(aisling, "Sword", 0)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawItemResult.BadInput);
    }

    [Test]
    public void WithdrawItem_ShouldReturnDontHaveThatMany_WhenBankDoesNotHaveItem()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.WithdrawItem(aisling, "Sword", 1)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawItemResult.DontHaveThatMany);
    }

    [Test]
    public void WithdrawItem_ShouldReturnCantCarry_WhenTooHeavy()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(0));
        var item = MockItem.Create("HeavySword");
        aisling.Bank.Deposit(item);

        ComplexActionHelper.WithdrawItem(aisling, "HeavySword", 1)
                           .Should()
                           .Be(ComplexActionHelper.WithdrawItemResult.CantCarry);
    }
    #endregion

    #region LearnSkill
    [Test]
    public void LearnSkill_ShouldSucceed_WhenSkillBookHasRoom()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var skill = MockSkill.Create();

        var result = ComplexActionHelper.LearnSkill(aisling, skill);

        result.Should()
              .Be(ComplexActionHelper.LearnSkillResult.Success);
    }

    [Test]
    public void LearnSkill_ShouldReturnNoRoom_WhenSkillBookIsFull()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        // Fill the skill book
        for (byte i = 1; i <= 89; i++)
            aisling.SkillBook.TryAddToNextSlot(MockSkill.Create($"Skill{i}"));

        var skill = MockSkill.Create("OneMore");

        ComplexActionHelper.LearnSkill(aisling, skill)
                           .Should()
                           .Be(ComplexActionHelper.LearnSkillResult.NoRoom);
    }
    #endregion

    #region LearnSpell
    [Test]
    public void LearnSpell_ShouldSucceed_WhenSpellBookHasRoom()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var spell = MockSpell.Create();

        var result = ComplexActionHelper.LearnSpell(aisling, spell);

        result.Should()
              .Be(ComplexActionHelper.LearnSpellResult.Success);
    }

    [Test]
    public void LearnSpell_ShouldReturnNoRoom_WhenSpellBookIsFull()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        // Fill the spell book
        for (byte i = 1; i <= 89; i++)
            aisling.SpellBook.TryAddToNextSlot(MockSpell.Create($"Spell{i}"));

        var spell = MockSpell.Create("OneMore");

        ComplexActionHelper.LearnSpell(aisling, spell)
                           .Should()
                           .Be(ComplexActionHelper.LearnSpellResult.NoRoom);
    }
    #endregion

    #region RemoveManyItems
    [Test]
    public void RemoveManyItems_ShouldSucceed_WhenAislingHasAllItems()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.Inventory.TryAddToNextSlot(MockItem.Create("Sword"));
        aisling.Inventory.TryAddToNextSlot(MockItem.Create("Shield"));

        var result = ComplexActionHelper.RemoveManyItems(aisling, ("Sword", 1), ("Shield", 1));

        result.Should()
              .Be(ComplexActionHelper.RemoveManyItemsResult.Success);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeFalse();

        aisling.Inventory
               .Contains("Shield")
               .Should()
               .BeFalse();
    }

    [Test]
    public void RemoveManyItems_ShouldReturnDontHaveThatMany_WhenInsufficientItems()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.Inventory.TryAddToNextSlot(MockItem.Create("Sword"));

        ComplexActionHelper.RemoveManyItems(aisling, ("Sword", 1), ("Shield", 1))
                           .Should()
                           .Be(ComplexActionHelper.RemoveManyItemsResult.DontHaveThatMany);
    }

    [Test]
    public void RemoveManyItems_ShouldSucceed_WhenEmptyList()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.RemoveManyItems(aisling, Array.Empty<(string, int)>())
                           .Should()
                           .Be(ComplexActionHelper.RemoveManyItemsResult.Success);
    }

    [Test]
    public void RemoveManyItems_ItemOverload_ShouldSucceed()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var sword = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(sword);

        var result = ComplexActionHelper.RemoveManyItems(aisling, sword);

        result.Should()
              .Be(ComplexActionHelper.RemoveManyItemsResult.Success);
    }
    #endregion

    #region BuyItem
    [Test]
    public void BuyItem_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();

        ComplexActionHelper.BuyItem(
                               aisling,
                               null,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               0,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.BadInput);
    }

    [Test]
    public void BuyItem_ShouldReturnBadInput_WhenCostIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();

        ComplexActionHelper.BuyItem(
                               aisling,
                               null,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               1,
                               -1)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.BadInput);
    }

    [Test]
    public void BuyItem_ShouldReturnCantCarry_WhenTooHeavy()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(0));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();

        ComplexActionHelper.BuyItem(
                               aisling,
                               null,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.CantCarry);
    }

    [Test]
    public void BuyItem_ShouldReturnNotEnoughGold_WhenInsufficientGold()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(50);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();

        itemFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<ISet<string>>()))
                       .Returns(MockItem.Create("Sword"));

        ComplexActionHelper.BuyItem(
                               aisling,
                               null,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.NotEnoughGold);
    }

    [Test]
    public void BuyItem_ShouldSucceed_WhenAllConditionsMet()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();

        itemFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<ISet<string>>()))
                       .Returns(MockItem.Create("Sword"));

        var result = ComplexActionHelper.BuyItem(
            aisling,
            null,
            fauxItem,
            itemFactoryMock.Object,
            MockScriptProvider.ItemCloner.Object,
            1,
            100);

        result.Should()
              .Be(ComplexActionHelper.BuyItemResult.Success);

        aisling.Gold
               .Should()
               .Be(900);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void BuyItem_ShouldReturnNotEnoughStock_WhenShopHasInsufficientStock()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();
        var shopMock = new Mock<IBuyShopSource>();

        shopMock.Setup(s => s.GetStock(It.IsAny<string>()))
                .Returns(0);

        ComplexActionHelper.BuyItem(
                               aisling,
                               shopMock.Object,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               5,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.NotEnoughStock);
    }

    [Test]
    public void BuyItem_ShouldReturnNotEnoughStock_WhenTryDecrementStockFails()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(1000);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();
        var shopMock = new Mock<IBuyShopSource>();

        // GetStock passes the fast check, but TryDecrementStock fails
        shopMock.Setup(s => s.GetStock(It.IsAny<string>()))
                .Returns(10);

        shopMock.Setup(s => s.TryDecrementStock(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

        ComplexActionHelper.BuyItem(
                               aisling,
                               shopMock.Object,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.NotEnoughStock);
    }

    [Test]
    public void BuyItem_ShouldReturnNotEnoughGold_AndRestoreStock_WhenShopPresentAndGoldInsufficient()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(50);

        var fauxItem = MockItem.Create("Sword");
        var itemFactoryMock = new Mock<IItemFactory>();
        var shopMock = new Mock<IBuyShopSource>();

        shopMock.Setup(s => s.GetStock(It.IsAny<string>()))
                .Returns(10);

        shopMock.Setup(s => s.TryDecrementStock(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

        itemFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<ISet<string>>()))
                       .Returns(MockItem.Create("Sword"));

        ComplexActionHelper.BuyItem(
                               aisling,
                               shopMock.Object,
                               fauxItem,
                               itemFactoryMock.Object,
                               MockScriptProvider.ItemCloner.Object,
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.BuyItemResult.NotEnoughGold);

        // Verify stock was restored (TryDecrementStock called with negative amount)
        shopMock.Verify(s => s.TryDecrementStock(It.IsAny<string>(), -1), Times.Once);
    }
    #endregion

    #region DepositItem (by name) - additional
    [Test]
    public void DepositItem_ByName_ShouldReturnBadInput_WhenAmountLessThan1()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(aisling, "Sword", 0)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnBadInput_WhenCostIsNegative()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        ComplexActionHelper.DepositItem(
                               aisling,
                               "Sword",
                               1,
                               -1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnBadInput_WhenPreventBanking()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("BoundItem", setup: i => i.PreventBanking = true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(aisling, "BoundItem", 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.BadInput);
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnDontHaveThatMany_WhenInsufficientQuantity()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Potion", 3, true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(aisling, "Potion", 10)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.DontHaveThatMany);
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnNotEnoughGold_WhenCantAffordCost()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.DepositItem(
                               aisling,
                               "Sword",
                               1,
                               100)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.NotEnoughGold);
    }

    [Test]
    public void DepositItem_ByName_ShouldReturnItemDamaged_WhenDurabilityLow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var durableItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);
        aisling.Inventory.TryAddToNextSlot(durableItem);

        ComplexActionHelper.DepositItem(aisling, "Sword", 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.ItemDamaged);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnItemDamaged_WhenDurabilityLow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var durableItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);
        aisling.Inventory.TryAddToNextSlot(durableItem);

        ComplexActionHelper.DepositItem(aisling, durableItem.Slot, 1)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.ItemDamaged);
    }
    #endregion

    #region SellItem (by slot) - additional
    [Test]
    public void SellItem_BySlot_ShouldReturnTooMuchGold_WhenGoldWouldOverflow()
    {
        // MaxGoldHeld is 10,000,000 in test setup
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(9_999_999);

        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.SellItem(
                               aisling,
                               item.Slot,
                               1,
                               10)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.TooMuchGold);
    }

    [Test]
    public void SellItem_BySlot_ShouldReturnItemDamaged_WhenDurabilityLow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var durableItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);
        aisling.Inventory.TryAddToNextSlot(durableItem);

        ComplexActionHelper.SellItem(
                               aisling,
                               durableItem.Slot,
                               1,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.ItemDamaged);
    }
    #endregion

    #region DepositItem — post-removal damaged item detection
    [Test]
    public void DepositItem_ByName_ShouldReturnItemDamaged_WhenSplitItemIsDamaged()
    {
        // First item is healthy (passes pre-check), second is damaged (caught by post-check)
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var healthyItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 100);

        var damagedItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);

        aisling.Inventory.TryAddDirect(1, healthyItem);
        aisling.Inventory.TryAddDirect(2, damagedItem);

        // Inventory["Sword"] returns healthyItem (first match) → passes pre-check
        // RemoveQuantity returns both → post-check catches damagedItem
        ComplexActionHelper.DepositItem(aisling, "Sword", 2)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.ItemDamaged);
    }

    [Test]
    public void DepositItem_BySlot_ShouldReturnItemDamaged_WhenSplitItemIsDamaged()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var healthyItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 100);

        var damagedItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);

        aisling.Inventory.TryAddDirect(1, healthyItem);
        aisling.Inventory.TryAddDirect(2, damagedItem);

        // RemoveQuantity(slot=1, qty=2) prepends slot 1 item, then finds slot 2 item
        // healthyItem passes pre-check, post-check catches damagedItem
        ComplexActionHelper.DepositItem(aisling, 1, 2)
                           .Should()
                           .Be(ComplexActionHelper.DepositItemResult.ItemDamaged);
    }
    #endregion

    #region SellItem — post-removal damaged item detection
    [Test]
    public void SellItem_BySlot_ShouldReturnItemDamaged_WhenSplitItemIsDamaged()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var healthyItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 100);

        var damagedItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);

        aisling.Inventory.TryAddDirect(1, healthyItem);
        aisling.Inventory.TryAddDirect(2, damagedItem);

        ComplexActionHelper.SellItem(
                               aisling,
                               1,
                               2,
                               10)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.ItemDamaged);

        // Gold should be reversed (TryTakeGold called after TryGiveGold)
        aisling.Gold
               .Should()
               .Be(0);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnItemDamaged_WhenSplitItemIsDamaged()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var healthyItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 100);

        var damagedItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);

        aisling.Inventory.TryAddDirect(1, healthyItem);
        aisling.Inventory.TryAddDirect(2, damagedItem);

        ComplexActionHelper.SellItem(
                               aisling,
                               "Sword",
                               2,
                               10)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.ItemDamaged);

        aisling.Gold
               .Should()
               .Be(0);
    }
    #endregion

    #region SellItem (by name) - additional
    [Test]
    public void SellItem_ByName_ShouldReturnDontHaveThatMany_WhenInsufficientQuantity()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        var item = MockItem.Create("Potion", 3, true);
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.SellItem(
                               aisling,
                               "Potion",
                               10,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.DontHaveThatMany);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnTooMuchGold_WhenGoldWouldOverflow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));
        aisling.TryGiveGold(9_999_999);

        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        ComplexActionHelper.SellItem(
                               aisling,
                               "Sword",
                               1,
                               10)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.TooMuchGold);
    }

    [Test]
    public void SellItem_ByName_ShouldReturnItemDamaged_WhenDurabilityLow()
    {
        var aisling = MockAisling.Create(setup: a => a.UserStatSheet.SetMaxWeight(100));

        var durableItem = MockItem.Create(
            "Sword",
            templateSetup: t => t with
            {
                MaxDurability = 100
            },
            setup: i => i.CurrentDurability = 50);
        aisling.Inventory.TryAddToNextSlot(durableItem);

        ComplexActionHelper.SellItem(
                               aisling,
                               "Sword",
                               1,
                               50)
                           .Should()
                           .Be(ComplexActionHelper.SellItemResult.ItemDamaged);
    }
    #endregion
}