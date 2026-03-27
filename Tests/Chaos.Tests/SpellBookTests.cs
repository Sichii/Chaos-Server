#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class SpellBookTests
{
    #region GetLastSlotInPage (SpellBook has Length 90)
    [Test]
    public void GetLastSlotInPage_Page3_ShouldReturn89()
    {
        var book = new SpellBook();

        // SpellBook length is 90, so last slot in page 3 is 89
        book.GetLastSlotInPage(PageType.Page3)
            .Should()
            .Be(89);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_Default_ShouldBeEmpty()
    {
        var book = new SpellBook();

        book.Count
            .Should()
            .Be(0);
    }

    [Test]
    public void Constructor_WithSpells_ShouldPopulate()
    {
        var spell = MockSpell.Create("Heal");
        spell.Slot = 1;

        var book = new SpellBook([spell]);

        book.Count
            .Should()
            .Be(1);

        book[1]
            .Should()
            .BeSameAs(spell);
    }
    #endregion

    #region TryAddToNextSlot
    [Test]
    public void TryAddToNextSlot_ShouldSkipInvalidSlots()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create($"Spell{Guid.NewGuid():N}");

        book.TryAddToNextSlot(spell);

        // Slot 0 is invalid, so should be placed at slot 1
        spell.Slot
             .Should()
             .Be(1);
    }

    [Test]
    public void TryAddToNextSlot_ShouldSkipSlot36And72()
    {
        var book = new SpellBook();

        // Fill slots 1-35 (page 1 valid slots)
        for (var i = 0; i < 35; i++)
            book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        // Next spell should go to slot 37, skipping slot 36
        var nextSpell = MockSpell.Create($"Spell{Guid.NewGuid():N}");
        book.TryAddToNextSlot(nextSpell);

        nextSpell.Slot
                 .Should()
                 .Be(37);
    }
    #endregion

    #region Contains / Remove
    [Test]
    public void Contains_ShouldFindByName()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create("Heal");
        book.TryAddToNextSlot(spell);

        book.Contains("Heal")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Remove_ShouldRemoveBySlot()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create("Heal");
        book.TryAddToNextSlot(spell);

        book.Remove(spell.Slot)
            .Should()
            .BeTrue();

        book.Count
            .Should()
            .Be(0);
    }
    #endregion

    #region IsFull
    [Test]
    public void IsFull_ShouldBeTrue_WhenAllValidSlotsFilled()
    {
        var book = new SpellBook();

        // SpellBook has 90 slots (0-89), with 3 invalid (0, 36, 72) = 87 valid slots
        for (var i = 0; i < 87; i++)
            book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        book.IsFull
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsFull_ShouldBeFalse_WhenSlotsAvailable()
    {
        var book = new SpellBook();
        book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        book.IsFull
            .Should()
            .BeFalse();
    }
    #endregion

    #region Contains(T obj)
    [Test]
    public void ContainsByObject_WhenPresent_ShouldReturnTrue()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create("Heal");
        book.TryAddToNextSlot(spell);

        book.Contains(spell)
            .Should()
            .BeTrue();
    }

    [Test]
    public void ContainsByObject_WhenNotPresent_ShouldReturnFalse()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create("Heal");

        book.Contains(spell)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsByObject_MatchesByTemplateName_CaseInsensitive()
    {
        var book = new SpellBook();
        var spell1 = MockSpell.Create("Heal");
        book.TryAddToNextSlot(spell1);

        var spell2 = MockSpell.Create("Heal");

        book.Contains(spell2)
            .Should()
            .BeTrue();
    }
    #endregion

    #region AvailableSlots
    [Test]
    public void AvailableSlots_EmptyBook_ShouldReturnTotalSlots()
    {
        var book = new SpellBook();

        // 87 valid slots (90 - 3 invalid: 0, 36, 72)
        book.AvailableSlots
            .Should()
            .Be(87);
    }

    [Test]
    public void AvailableSlots_AfterAdding_ShouldDecrease()
    {
        var book = new SpellBook();
        book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));
        book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        book.AvailableSlots
            .Should()
            .Be(85);
    }
    #endregion

    #region TrySwap edge cases
    [Test]
    public void TrySwap_BothSlotsEmpty_ShouldReturnTrue()
    {
        var book = new SpellBook();

        book.TrySwap(1, 2)
            .Should()
            .BeTrue();

        book[1]
            .Should()
            .BeNull();

        book[2]
            .Should()
            .BeNull();
    }

    [Test]
    public void TrySwap_OneSlotNull_ShouldMoveSpell()
    {
        var book = new SpellBook();
        var spell = MockSpell.Create("Heal");
        book.TryAdd(3, spell);

        book.TrySwap(3, 10)
            .Should()
            .BeTrue();

        book[3]
            .Should()
            .BeNull();

        book[10]
            .Should()
            .BeSameAs(spell);

        spell.Slot
             .Should()
             .Be(10);
    }
    #endregion

    #region TryAddToNextSlot(PageType) edge cases
    [Test]
    public void TryAddToNextSlotByPage_PageFull_ShouldFallBackToAnySlot()
    {
        var book = new SpellBook();

        // Fill page 1 (slots 1-35 = 35 spells)
        for (var i = 0; i < 35; i++)
            book.TryAddToNextSlot(PageType.Page1, MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        // Page 1 is full, so the next add should fall back to any available slot
        var spell = MockSpell.Create("Overflow");
        var result = book.TryAddToNextSlot(PageType.Page1, spell);

        result.Should()
              .BeTrue();

        // Should have ended up on page 2 or 3 (slot >= 37)
        spell.Slot
             .Should()
             .BeGreaterThanOrEqualTo(37);
    }

    [Test]
    public void TryAddToNextSlotByPage_WhenBookIsFull_ShouldReturnFalse()
    {
        var book = new SpellBook();

        // Fill all 87 valid slots
        for (var i = 0; i < 87; i++)
            book.TryAddToNextSlot(MockSpell.Create($"Spell{Guid.NewGuid():N}"));

        var spell = MockSpell.Create($"Spell{Guid.NewGuid():N}");

        book.TryAddToNextSlot(PageType.Page2, spell)
            .Should()
            .BeFalse();
    }
    #endregion
}