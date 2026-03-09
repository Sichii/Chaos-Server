#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.TypeMapper.Abstractions;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class BankTests
{
    private readonly Mock<ICloningService<Item>> ClonerMock;
    private readonly Mock<IScriptProvider> ScriptProviderMock;

    public BankTests()
    {
        ScriptProviderMock = new Mock<IScriptProvider>();

        ScriptProviderMock.Setup(sp => sp.CreateScript<IItemScript, Item>(It.IsAny<ICollection<string>>(), It.IsAny<Item>()))
                          .Returns(new Mock<IItemScript>().Object);

        ClonerMock = new Mock<ICloningService<Item>>();

        ClonerMock.Setup(c => c.Clone(It.IsAny<Item>()))
                  .Returns<Item>(original =>
                  {
                      var clone = new Item(original.Template, ScriptProviderMock.Object);
                      clone.Count = original.Count;

                      return clone;
                  });
    }

    #region Construction with initial items
    [Test]
    public void Bank_ShouldAcceptInitialItems()
    {
        var items = new[]
        {
            CreateItem("Sword"),
            CreateItem("Shield")
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
        var item = CreateItem("Sword");

        bank.Deposit(item);

        bank.Contains("Sword")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Deposit_ShouldThrow_WhenItemCountIsZero()
    {
        var bank = CreateBank();
        var item = CreateItem("Sword");
        item.Count = 0;

        var act = () => bank.Deposit(item);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Deposit_ShouldConsolidateStacks_WhenSameItemDeposited()
    {
        var bank = CreateBank();
        var item1 = CreateItem("Potion", 5);
        var item2 = CreateItem("Potion", 3);

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
        var item1 = CreateItem("Potion", 5);
        var item2 = CreateItem("Potion", 3);

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
        bank.Deposit(CreateItem("Sword"));

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
        bank.Deposit(CreateItem("Potion", 7));

        bank.CountOf("Potion")
            .Should()
            .Be(7);
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenSufficientCount()
    {
        var bank = CreateBank();
        bank.Deposit(CreateItem("Potion", 10));

        bank.HasCount("Potion", 5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnTrue_WhenExactCount()
    {
        var bank = CreateBank();
        bank.Deposit(CreateItem("Potion", 5));

        bank.HasCount("Potion", 5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasCount_ShouldReturnFalse_WhenInsufficientCount()
    {
        var bank = CreateBank();
        bank.Deposit(CreateItem("Potion", 3));

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

        var result = bank.TryWithdraw("Nonexistent", 1, out var items);

        result.Should()
              .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void TryWithdraw_ShouldReturnFalse_WhenInsufficientCount()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(CreateItem("Potion", 3));

        var result = bank.TryWithdraw("Potion", 10, out var items);

        result.Should()
              .BeFalse();

        items.Should()
             .BeNull();
    }

    [Test]
    public void TryWithdraw_ShouldRemoveItemFromBank_WhenWithdrawingExactAmount()
    {
        var bank = CreateBankWithCloner();
        bank.Deposit(CreateItem("Potion", 5));

        var result = bank.TryWithdraw("Potion", 5, out var items);

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
        bank.Deposit(CreateItem("Potion", 10, true));

        var result = bank.TryWithdraw("Potion", 3, out var items);

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
        bank.Deposit(CreateItem("Potion", 5));

        var act = () => bank.TryWithdraw("Potion", 0, out _);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region Enumeration
    [Test]
    public void GetEnumerator_ShouldReturnAllItems()
    {
        var bank = CreateBank();
        bank.Deposit(CreateItem("Sword"));
        bank.Deposit(CreateItem("Shield"));
        bank.Deposit(CreateItem("Potion"));

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

    private Bank CreateBankWithCloner() => new(null, ClonerMock.Object);

    private Item CreateItem(string name, int count = 1, bool stackable = false)
    {
        var template = new ItemTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            ItemSprite = new ItemSprite(1, 1),
            PanelSprite = 1,
            Color = DisplayColor.Default,
            PantsColor = DisplayColor.Default,
            MaxStacks = stackable ? 100 : 1,
            BuyCost = 0,
            SellValue = 0,
            Category = "test",
            Description = null,
            EquipmentType = null,
            Gender = null,
            Class = null,
            AdvClass = null,
            IsDyeable = false,
            IsModifiable = false,
            NoTrade = false,
            AccountBound = false,
            PreventBanking = false,
            Level = 1,
            AbilityLevel = 0,
            MaxDurability = null,
            Modifiers = null,
            Weight = 1,
            Cooldown = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var item = new Item(template, ScriptProviderMock.Object);
        item.Count = count;

        return item;
    }
    #endregion
}