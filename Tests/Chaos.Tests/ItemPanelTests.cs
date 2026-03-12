#region
using Chaos.Collections;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

/// <summary>
///     Tests for Item-specific methods (FixStacks, Split, ToAmountString, Use, Update)
/// </summary>
public sealed class ItemPanelTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region IDialogSourceEntity.Activate
    [Test]
    public void Activate_ShouldCallScriptOnUse_ViaDialogSourceEntityInterface()
    {
        var item = MockItem.Create();
        var aisling = MockAisling.Create(Map);

        ((IDialogSourceEntity)item).Activate(aisling);

        Mock.Get(item.Script)
            .Verify(s => s.OnUse(aisling), Times.AtLeastOnce);
    }
    #endregion

    #region Constructor — ExtraScriptKeys Branch
    [Test]
    public void Constructor_ShouldAddExtraScriptKeys_WhenProvided()
    {
        var item = MockItem.Create(
            "ScriptedItem",
            templateSetup: t => t with
            {
                ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "BaseScript"
                }
            });

        // Need to create a fresh item with extra script keys through the Item constructor directly
        var template = item.Template;

        var itemWithExtras = new Item(template, MockScriptProvider.Instance.Object, ["ExtraScript"]);

        itemWithExtras.ScriptKeys
                      .Should()
                      .Contain("BaseScript");

        itemWithExtras.ScriptKeys
                      .Should()
                      .Contain("ExtraScript");
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldCallBaseUpdateAndScriptUpdate()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.Zero;

        var delta = TimeSpan.FromSeconds(1);

        item.Update(delta);

        // Base update should have incremented Elapsed
        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(1));

        // Script.Update should have been called
        Mock.Get(item.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }
    #endregion

    #region Constructor — Modifiers Branch
    [Test]
    public void Constructor_ShouldShallowCopyModifiers_WhenTemplateModifiersNotNull()
    {
        var templateModifiers = new Attributes
        {
            Str = 5,
            Int = 3
        };

        var item = MockItem.Create(
            "ModdedSword",
            templateSetup: t => t with
            {
                Modifiers = templateModifiers
            });

        item.Modifiers
            .Should()
            .NotBeNull();

        item.Modifiers
            .Str
            .Should()
            .Be(5);

        item.Modifiers
            .Int
            .Should()
            .Be(3);

        // Verify it's a copy, not the same reference
        item.Modifiers
            .Should()
            .NotBeSameAs(templateModifiers);
    }

    [Test]
    public void Constructor_ShouldCreateEmptyModifiers_WhenTemplateModifiersNull()
    {
        var item = MockItem.Create("PlainSword");

        item.Modifiers
            .Should()
            .NotBeNull();

        item.Modifiers
            .Str
            .Should()
            .Be(0);
    }
    #endregion

    #region FixStacks
    [Test]
    public void FixStacks_ShouldReturnSelf_WhenCountIsLessThanOrEqualToMaxStacks()
    {
        var item = MockItem.Create("Potion", 50, true);

        var result = item.FixStacks(MockScriptProvider.ItemCloner.Object)
                         .ToList();

        result.Should()
              .HaveCount(1);

        result[0]
            .Should()
            .BeSameAs(item);
    }

    [Test]
    public void FixStacks_ShouldReturnExactMultipleFullStacks_WhenCountIsExactMultiple()
    {
        // MaxStacks is 100 for stackable items
        var item = MockItem.Create("Potion", 300, true);

        var result = item.FixStacks(MockScriptProvider.ItemCloner.Object)
                         .ToList();

        result.Should()
              .HaveCount(3);

        result.Should()
              .AllSatisfy(clone => clone.Count
                                        .Should()
                                        .Be(100));
    }

    [Test]
    public void FixStacks_ShouldReturnFullStacksPlusRemainder_WhenCountIsNotExactMultiple()
    {
        var item = MockItem.Create("Potion", 250, true);

        var result = item.FixStacks(MockScriptProvider.ItemCloner.Object)
                         .ToList();

        result.Should()
              .HaveCount(3);

        result[0]
            .Count
            .Should()
            .Be(100);

        result[1]
            .Count
            .Should()
            .Be(100);

        result[2]
            .Count
            .Should()
            .Be(50);
    }

    [Test]
    public void FixStacks_ShouldReturnSelf_WhenCountEqualsMaxStacks()
    {
        var item = MockItem.Create("Potion", 100, true);

        var result = item.FixStacks(MockScriptProvider.ItemCloner.Object)
                         .ToList();

        result.Should()
              .HaveCount(1);

        result[0]
            .Should()
            .BeSameAs(item);
    }

    [Test]
    public void FixStacks_ShouldReturnSelf_WhenCountIsZero()
    {
        var item = MockItem.Create("Potion", 0, true);

        var result = item.FixStacks(MockScriptProvider.ItemCloner.Object)
                         .ToList();

        result.Should()
              .HaveCount(1);

        result[0]
            .Should()
            .BeSameAs(item);
    }
    #endregion

    #region Split
    [Test]
    public void Split_ShouldReduceCountAndReturnClone_WhenValid()
    {
        var item = MockItem.Create("Potion", 10, true);

        var clone = item.Split(3, MockScriptProvider.ItemCloner.Object);

        item.Count
            .Should()
            .Be(7);

        clone.Count
             .Should()
             .Be(3);
    }

    [Test]
    public void Split_ShouldThrowInvalidOperationException_WhenCountLessThanOrEqualToSplitAmount()
    {
        var item = MockItem.Create("Potion", 5, true);

        var act = () => item.Split(5, MockScriptProvider.ItemCloner.Object);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void Split_ShouldThrowInvalidOperationException_WhenCountIsLessThanSplitAmount()
    {
        var item = MockItem.Create("Potion", 3, true);

        var act = () => item.Split(5, MockScriptProvider.ItemCloner.Object);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void Split_ShouldThrowInvalidOperationException_WhenItemIsNotStackable()
    {
        var item = MockItem.Create("Sword", 5);

        var act = () => item.Split(2, MockScriptProvider.ItemCloner.Object);

        act.Should()
           .Throw<InvalidOperationException>();
    }
    #endregion

    #region ToAmountString
    [Test]
    public void ToAmountString_ShouldReturnPluralForm_WhenAmountGreaterThanOne()
    {
        var item = MockItem.Create("Sword");

        var result = item.ToAmountString(3);

        result.Should()
              .Be("3 Swords");
    }

    [Test]
    public void ToAmountString_ShouldReturnSingularForm_WhenAmountIsOne()
    {
        var item = MockItem.Create("Sword");

        var result = item.ToAmountString(1);

        result.Should()
              .Be("1 Sword");
    }

    [Test]
    public void ToAmountString_ShouldReturnSingularForm_WhenAmountIsZero()
    {
        var item = MockItem.Create("Sword");

        var result = item.ToAmountString(0);

        result.Should()
              .Be("0 Sword");
    }
    #endregion

    #region Use
    [Test]
    public void Use_ShouldCallScriptOnUseAndBeginCooldown()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);

        var aisling = MockAisling.Create(Map);

        item.Use(aisling);

        Mock.Get(item.Script)
            .Verify(s => s.OnUse(aisling), Times.AtLeastOnce);

        // BeginCooldown should have been called, setting Elapsed
        item.Elapsed
            .Should()
            .Be(TimeSpan.Zero);
    }

    [Test]
    public void Use_ShouldSendCooldownToAislingClient()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);

        var aisling = MockAisling.Create(Map);

        item.Use(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(item), Times.Once);
    }
    #endregion
}