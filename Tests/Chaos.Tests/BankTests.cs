#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class BankTests
{
    #region Construction with initial items
    [Test]
    public void Bank_ShouldAcceptInitialItems()
    {
        var items = new[]
        {
            MockItem.Create("Sword"),
            MockItem.Create("Shield")
        };

        var bank = new Bank(items);

        bank.Contains("Sword")
            .Should()
            .BeTrue();

        bank.Contains("Shield")
            .Should()
            .BeTrue();
    }
    #endregion

    #region Gold
    [Test]
    public void AddGold_ShouldIncreaseGold()
    {
        var bank = CreateBank();

        bank.AddGold(100);

        bank.Gold
            .Should()
            .Be(100);
    }

    [Test]
    public void AddGold_MultipleTimes_ShouldAccumulate()
    {
        var bank = CreateBank();

        bank.AddGold(100);
        bank.AddGold(200);

        bank.Gold
            .Should()
            .Be(300);
    }

    [Test]
    public void RemoveGold_ShouldReturnTrue_WhenSufficientGold()
    {
        var bank = CreateBank();
        bank.AddGold(100);

        var result = bank.RemoveGold(50);

        result.Should()
              .BeTrue();

        bank.Gold
            .Should()
            .Be(50);
    }

    [Test]
    public void RemoveGold_ShouldReturnFalse_WhenInsufficientGold()
    {
        var bank = CreateBank();
        bank.AddGold(50);

        var result = bank.RemoveGold(100);

        result.Should()
              .BeFalse();

        bank.Gold
            .Should()
            .Be(50);
    }

    [Test]
    public void RemoveGold_ShouldReturnTrue_WhenExactAmount()
    {
        var bank = CreateBank();
        bank.AddGold(100);

        var result = bank.RemoveGold(100);

        result.Should()
              .BeTrue();

        bank.Gold
            .Should()
            .Be(0);
    }

    [Test]
    public void RemoveGold_ShouldReturnFalse_WhenBankIsEmpty()
    {
        var bank = CreateBank();

        var result = bank.RemoveGold(1);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Deposit / Contains / CountOf / HasCount
    [Test]
    public void Deposit_ShouldAddItemToBank()
    {
        var bank = CreateBank();
        var item = MockItem.Create("Sword");

        bank.Deposit(item);

        bank.Contains("Sword")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Deposit_ShouldThrow_WhenItemCountIsZero()
    {
        var bank = CreateBank();
        var item = MockItem.Create("Sword");
        item.Count = 0;

        var act = () => bank.Deposit(item);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Deposit_ShouldConsolidateStacks_WhenSameItemDeposited()
    {
        var bank = CreateBank();
        var item1 = MockItem.Create("Potion", 5);
        var item2 = MockItem.Create("Potion", 3);

        bank.Deposit(item1);
        bank.Deposit(item2);

        bank.CountOf("Potion")
            .Should()
            .Be(8);
    }

    [Test]
    public void Deposit_ShouldZeroOriginalItemCount_WhenConsolidated()
    {
        var bank = CreateBank();
        var item1 = MockItem.Create("Potion", 5);
        var item2 = MockItem.Create("Potion", 3);

        bank.Deposit(item1);
        bank.Deposit(item2);

        // When consolidated into existing stack, the deposited item's count is zeroed
        item2.Count
             .Should()
             .Be(0);
    }

    [Test]
    public void Contains_ShouldReturnFalse_WhenItemNotInBank()
    {
        var bank = CreateBank();

        bank.Contains("Nonexistent")
            .Should()
            .BeFalse();
    }

    [Test]
    public void Contains_ShouldBeCaseInsensitive()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Sword"));

        bank.Contains("sword")
            .Should()
            .BeTrue();

        bank.Contains("SWORD")
            .Should()
            .BeTrue();
    }

    [Test]
    public void CountOf_ShouldReturnZero_WhenItemNotInBank()
    {
        var bank = CreateBank();

        bank.CountOf("Nonexistent")
            .Should()
            .Be(0);
    }

    [Test]
    public void CountOf_ShouldReturnItemCount()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Potion", 7));

        bank.CountOf("Potion")
            .Should()
            .Be(7);
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenSufficientCount()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Potion", 10));

        bank.HasCount("Potion", 5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenExactCount()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Potion", 5));

        bank.HasCount("Potion", 5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnFalse_WhenInsufficientCount()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Potion", 3));

        bank.HasCount("Potion", 5)
            .Should()
            .BeFalse();
    }
    #endregion

    #region TryWithdraw
    [Test]
    public void TryWithdraw_ShouldReturnFalse_WhenItemNotInBank()
    {
        var bank = CreateBankWithCloner();

        var result = bank.TryRemove("Nonexistent", 1, out var items);

        result.Should()
              .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void TryWithdraw_ShouldReturnFalse_WhenInsufficientCount()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(MockItem.Create("Potion", 3));

        var result = bank.TryRemove("Potion", 10, out var items);

        result.Should()
              .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void TryWithdraw_ShouldRemoveItemFromBank_WhenWithdrawingExactAmount()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(MockItem.Create("Potion", 5));

        var result = bank.TryRemove("Potion", 5, out var items);

        result.Should()
              .BeTrue();

        items.Should()
             .NotBeNull();

        bank.Contains("Potion")
            .Should()
            .BeFalse();
    }

    [Test]
    public void TryWithdraw_ShouldReduceCount_WhenPartialWithdraw()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(MockItem.Create("Potion", 10, true));

        var result = bank.TryRemove("Potion", 3, out var items);

        result.Should()
              .BeTrue();

        items.Should()
             .NotBeNull();

        // Remaining in bank
        bank.CountOf("Potion")
            .Should()
            .Be(7);
    }

    [Test]
    public void TryWithdraw_ShouldThrow_WhenAmountIsZero()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(MockItem.Create("Potion", 5));

        var act = () => bank.TryRemove("Potion", 0, out _);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region Enumeration
    [Test]
    public void GetEnumerator_ShouldReturnAllItems()
    {
        var bank = CreateBank();
        bank.Deposit(MockItem.Create("Sword"));
        bank.Deposit(MockItem.Create("Shield"));
        bank.Deposit(MockItem.Create("Potion"));

        var items = bank.ToList();

        items.Should()
             .HaveCount(3);
    }

    [Test]
    public void GetEnumerator_ShouldReturnEmpty_WhenBankIsEmpty()
    {
        var bank = CreateBank();

        var items = bank.ToList();

        items.Should()
             .BeEmpty();
    }
    #endregion

    #region Helpers
    private Bank CreateBank() => new();

    private Bank CreateBankWithCloner() => new(null, MockScriptProvider.ItemCloner.Object);
    #endregion
}