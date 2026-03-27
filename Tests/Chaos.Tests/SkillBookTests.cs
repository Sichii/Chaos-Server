#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class SkillBookTests
{
    #region TrySwap
    [Test]
    public void TrySwap_ShouldSwapTwoSkills()
    {
        var book = new SkillBook();
        var skill1 = MockSkill.Create("Skill1");
        var skill2 = MockSkill.Create("Skill2");
        book.TryAddToNextSlot(skill1);
        book.TryAddToNextSlot(skill2);

        var slot1 = skill1.Slot;
        var slot2 = skill2.Slot;

        book.TrySwap(slot1, slot2)
            .Should()
            .BeTrue();

        book[slot1]!.Template
                    .Name
                    .Should()
                    .Be("Skill2");

        book[slot2]!.Template
                    .Name
                    .Should()
                    .Be("Skill1");
    }
    #endregion

    #region Update(TimeSpan)
    [Test]
    public void UpdateTimeSpan_ShouldCallUpdateOnAllItems()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        skill.Elapsed = TimeSpan.Zero;
        skill.Cooldown = TimeSpan.FromSeconds(10);
        book.TryAddToNextSlot(skill);

        book.Update(TimeSpan.FromSeconds(1));

        // After update, Elapsed should have advanced
        skill.Elapsed
             .Should()
             .Be(TimeSpan.FromSeconds(1));
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_Default_ShouldBeEmpty()
    {
        var book = new SkillBook();

        book.Count
            .Should()
            .Be(0);
    }

    [Test]
    public void Constructor_WithSkills_ShouldPopulate()
    {
        var skill = MockSkill.Create("Assail");
        skill.Slot = 1;

        var book = new SkillBook([skill]);

        book.Count
            .Should()
            .Be(1);

        book[1]
            .Should()
            .BeSameAs(skill);
    }
    #endregion

    #region TryAddToNextSlot
    [Test]
    public void TryAddToNextSlot_ShouldSkipInvalidSlots()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAddToNextSlot(skill);

        // Slot 0 is invalid, so should be placed at slot 1
        skill.Slot
             .Should()
             .Be(1);
    }

    [Test]
    public void TryAddToNextSlot_ShouldSkipSlot36And72()
    {
        var book = new SkillBook();

        // Fill slots 1-35 (page 1 valid slots)
        for (var i = 0; i < 35; i++)
            book.TryAddToNextSlot(MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        // Next skill should go to slot 37, skipping slot 36
        var nextSkill = MockSkill.Create($"Skill{Guid.NewGuid():N}");
        book.TryAddToNextSlot(nextSkill);

        nextSkill.Slot
                 .Should()
                 .Be(37);
    }
    #endregion

    #region TryAddToNextSlot(PageType)
    [Test]
    public void TryAddToNextSlot_Page2_ShouldStartAtSlot37()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAddToNextSlot(PageType.Page2, skill);

        skill.Slot
             .Should()
             .Be(37);
    }

    [Test]
    public void TryAddToNextSlot_Page3_ShouldStartAtSlot73()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAddToNextSlot(PageType.Page3, skill);

        skill.Slot
             .Should()
             .Be(73);
    }
    #endregion

    #region Contains / Remove
    [Test]
    public void Contains_ShouldFindByName()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.Contains("Strike")
            .Should()
            .BeTrue();
    }

    [Test]
    public void Remove_ShouldRemoveBySlot()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.Remove(skill.Slot)
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
        var book = new SkillBook();

        // SkillBook has 89 slots (0-88), with 3 invalid (0, 36, 72) = 86 valid slots
        for (var i = 0; i < 86; i++)
            book.TryAddToNextSlot(MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        book.IsFull
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsFull_ShouldBeFalse_WhenSlotsAvailable()
    {
        var book = new SkillBook();
        book.TryAddToNextSlot(MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        book.IsFull
            .Should()
            .BeFalse();
    }
    #endregion

    #region IsValidSlot
    [Test]
    public void IsValidSlot_Slot0_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.IsValidSlot(0)
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsValidSlot_SlotAtLength_ShouldReturnFalse()
    {
        var book = new SkillBook();

        // SkillBook length is 89, so slot 89 is out of range
        book.IsValidSlot(89)
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsValidSlot_SlotBeyondLength_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.IsValidSlot(100)
            .Should()
            .BeFalse();
    }

    //formatter:off
    [Test]
    [Arguments((byte)36)]
    [Arguments((byte)72)]

    //formatter:on
    public void IsValidSlot_InvalidSlots_ShouldReturnFalse(byte slot)
    {
        var book = new SkillBook();

        book.IsValidSlot(slot)
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsValidSlot_ValidSlot_ShouldReturnTrue()
    {
        var book = new SkillBook();

        book.IsValidSlot(1)
            .Should()
            .BeTrue();
    }
    #endregion

    #region Contains(byte slot)
    [Test]
    public void ContainsBySlot_InvalidSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.Contains(0)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsBySlot_ValidEmptySlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.Contains(1)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsBySlot_ValidOccupiedSlot_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");
        book.TryAddToNextSlot(skill);

        book.Contains(skill.Slot)
            .Should()
            .BeTrue();
    }
    #endregion

    #region ContainsByTemplateKey
    [Test]
    public void ContainsByTemplateKey_Found_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.ContainsByTemplateKey("strike")
            .Should()
            .BeTrue();
    }

    [Test]
    public void ContainsByTemplateKey_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.ContainsByTemplateKey("nonexistent")
            .Should()
            .BeFalse();
    }
    #endregion

    #region Indexer[byte slot]
    [Test]
    public void IndexerBySlot_InvalidSlot_ShouldReturnNull()
    {
        var book = new SkillBook();

        book[0]
            .Should()
            .BeNull();
    }

    [Test]
    public void IndexerBySlot_ValidEmptySlot_ShouldReturnNull()
    {
        var book = new SkillBook();

        book[5]
            .Should()
            .BeNull();
    }

    [Test]
    public void IndexerBySlot_ValidOccupiedSlot_ShouldReturnSkill()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book[skill.Slot]
            .Should()
            .BeSameAs(skill);
    }
    #endregion

    #region Indexer[string name]
    [Test]
    public void IndexerByName_Found_ShouldReturnSkill()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book["Strike"]
            .Should()
            .BeSameAs(skill);
    }

    [Test]
    public void IndexerByName_NotFound_ShouldReturnNull()
    {
        var book = new SkillBook();

        book["NonExistent"]
            .Should()
            .BeNull();
    }
    #endregion

    #region Remove(string name)
    [Test]
    public void RemoveByName_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.Remove("NonExistent")
            .Should()
            .BeFalse();
    }

    [Test]
    public void RemoveByName_Found_ShouldReturnTrueAndRemove()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.Remove("Strike")
            .Should()
            .BeTrue();

        book.Count
            .Should()
            .Be(0);
    }
    #endregion

    #region Remove(byte slot) edge cases
    [Test]
    public void RemoveBySlot_InvalidSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.Remove(0)
            .Should()
            .BeFalse();
    }

    [Test]
    public void RemoveBySlot_EmptySlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.Remove(5)
            .Should()
            .BeFalse();
    }
    #endregion

    #region RemoveByTemplateKey
    [Test]
    public void RemoveByTemplateKey_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.RemoveByTemplateKey("nonexistent")
            .Should()
            .BeFalse();
    }

    [Test]
    public void RemoveByTemplateKey_Found_ShouldReturnTrueAndRemove()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.RemoveByTemplateKey("strike")
            .Should()
            .BeTrue();

        book.Count
            .Should()
            .Be(0);
    }
    #endregion

    #region TryAdd(byte slot, T obj)
    [Test]
    public void TryAdd_InvalidSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAdd(0, skill)
            .Should()
            .BeFalse();
    }

    [Test]
    public void TryAdd_OccupiedSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();
        var skill1 = MockSkill.Create("Skill1");
        book.TryAdd(1, skill1);

        var skill2 = MockSkill.Create("Skill2");

        book.TryAdd(1, skill2)
            .Should()
            .BeFalse();
    }

    [Test]
    public void TryAdd_ValidEmptySlot_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAdd(5, skill)
            .Should()
            .BeTrue();

        skill.Slot
             .Should()
             .Be(5);
    }
    #endregion

    #region TryGetObject(byte slot)
    [Test]
    public void TryGetObjectBySlot_InvalidSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetObject(0, out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetObjectBySlot_EmptySlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetObject(5, out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetObjectBySlot_OccupiedSlot_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.TryGetObject(skill.Slot, out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);
    }
    #endregion

    #region TryGetObject(string name)
    [Test]
    public void TryGetObjectByName_Found_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.TryGetObject("Strike", out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);
    }

    [Test]
    public void TryGetObjectByName_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetObject("NonExistent", out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }
    #endregion

    #region TryGetObjectByTemplateKey
    [Test]
    public void TryGetObjectByTemplateKey_Found_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.TryGetObjectByTemplateKey("strike", out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);
    }

    [Test]
    public void TryGetObjectByTemplateKey_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetObjectByTemplateKey("nonexistent", out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }
    #endregion

    #region TryGetRemove(byte slot)
    [Test]
    public void TryGetRemoveBySlot_InvalidSlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetRemove(0, out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetRemoveBySlot_EmptySlot_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetRemove(5, out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetRemoveBySlot_OccupiedSlot_ShouldReturnTrueAndRemove()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);
        var slot = skill.Slot;

        book.TryGetRemove(slot, out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);

        book.Contains(slot)
            .Should()
            .BeFalse();
    }
    #endregion

    #region TryGetRemove(string name)
    [Test]
    public void TryGetRemoveByName_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetRemove("NonExistent", out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetRemoveByName_Found_ShouldReturnTrueAndRemove()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.TryGetRemove("Strike", out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);

        book.Count
            .Should()
            .Be(0);
    }
    #endregion

    #region TryGetRemoveByTemplateKey
    [Test]
    public void TryGetRemoveByTemplateKey_NotFound_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TryGetRemoveByTemplateKey("nonexistent", out var result)
            .Should()
            .BeFalse();

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryGetRemoveByTemplateKey_Found_ShouldReturnTrueAndRemove()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.TryGetRemoveByTemplateKey("strike", out var result)
            .Should()
            .BeTrue();

        result.Should()
              .BeSameAs(skill);

        book.Count
            .Should()
            .Be(0);
    }
    #endregion

    #region TrySwap edge cases
    [Test]
    public void TrySwap_InvalidSlot1_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TrySwap(0, 1)
            .Should()
            .BeFalse();
    }

    [Test]
    public void TrySwap_InvalidSlot2_ShouldReturnFalse()
    {
        var book = new SkillBook();

        book.TrySwap(1, 0)
            .Should()
            .BeFalse();
    }

    [Test]
    public void TrySwap_OneSlotNull_ShouldSwap()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAdd(1, skill);

        book.TrySwap(1, 2)
            .Should()
            .BeTrue();

        book[1]
            .Should()
            .BeNull();

        book[2]
            .Should()
            .BeSameAs(skill);

        skill.Slot
             .Should()
             .Be(2);
    }

    [Test]
    public void TrySwap_BothSlotsNull_ShouldReturnTrue()
    {
        var book = new SkillBook();

        book.TrySwap(1, 2)
            .Should()
            .BeTrue();
    }
    #endregion

    #region Update(slot, action)
    [Test]
    public void UpdateSlot_InvalidSlot_ShouldNotThrow()
    {
        var book = new SkillBook();
        var invoked = false;

        book.Update(0, _ => invoked = true);

        invoked.Should()
               .BeFalse();
    }

    [Test]
    public void UpdateSlot_EmptySlot_ShouldNotInvokeAction()
    {
        var book = new SkillBook();
        var invoked = false;

        book.Update(5, _ => invoked = true);

        invoked.Should()
               .BeFalse();
    }

    [Test]
    public void UpdateSlot_OccupiedSlot_WithAction_ShouldInvokeAction()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);
        var invoked = false;

        book.Update(skill.Slot, _ => invoked = true);

        invoked.Should()
               .BeTrue();
    }

    [Test]
    public void UpdateSlot_OccupiedSlot_WithoutAction_ShouldNotThrow()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        var act = () => book.Update(skill.Slot);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region GetFirstSlotInPage / GetLastSlotInPage
    [Test]
    public void GetFirstSlotInPage_InvalidPage_ShouldThrow()
    {
        var book = new SkillBook();

        var act = () => book.GetFirstSlotInPage((PageType)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GetLastSlotInPage_InvalidPage_ShouldThrow()
    {
        var book = new SkillBook();

        var act = () => book.GetLastSlotInPage((PageType)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    //formatter:off
    [Test]
    [Arguments(PageType.Page1, (byte)0)]
    [Arguments(PageType.Page2, (byte)37)]
    [Arguments(PageType.Page3, (byte)73)]

    //formatter:on
    public void GetFirstSlotInPage_ValidPages_ShouldReturnExpected(PageType page, byte expected)
    {
        var book = new SkillBook();

        book.GetFirstSlotInPage(page)
            .Should()
            .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(PageType.Page1, (byte)35)]
    [Arguments(PageType.Page2, (byte)71)]
    [Arguments(PageType.Page3, (byte)88)]

    //formatter:on
    public void GetLastSlotInPage_ValidPages_ShouldReturnExpected(PageType page, byte expected)
    {
        var book = new SkillBook();

        book.GetLastSlotInPage(page)
            .Should()
            .Be(expected);
    }
    #endregion

    #region TryAddToNextSlot(PageType) edge cases
    [Test]
    public void TryAddToNextSlotByPage_WhenBookIsFull_ShouldReturnFalse()
    {
        var book = new SkillBook();

        // Fill all 86 valid slots
        for (var i = 0; i < 86; i++)
            book.TryAddToNextSlot(MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        var skill = MockSkill.Create($"Skill{Guid.NewGuid():N}");

        book.TryAddToNextSlot(PageType.Page1, skill)
            .Should()
            .BeFalse();
    }

    [Test]
    public void TryAddToNextSlotByPage_PageFull_ShouldFallBackToAnySlot()
    {
        var book = new SkillBook();

        // Fill page 1 (slots 1-35 = 35 skills)
        for (var i = 0; i < 35; i++)
            book.TryAddToNextSlot(PageType.Page1, MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        // Page 1 is full, so the next add should fall back to any available slot
        var skill = MockSkill.Create("Overflow");
        var result = book.TryAddToNextSlot(PageType.Page1, skill);

        result.Should()
              .BeTrue();

        // Should have ended up on page 2 or 3 (not page 1)
        skill.Slot
             .Should()
             .BeGreaterThanOrEqualTo(37);
    }
    #endregion

    #region Contains(T obj)
    [Test]
    public void ContainsByObject_WhenPresent_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill);

        book.Contains(skill)
            .Should()
            .BeTrue();
    }

    [Test]
    public void ContainsByObject_WhenNotPresent_ShouldReturnFalse()
    {
        var book = new SkillBook();
        var skill = MockSkill.Create("Strike");

        book.Contains(skill)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsByObject_MatchesByTemplateName_ShouldReturnTrue()
    {
        var book = new SkillBook();
        var skill1 = MockSkill.Create("Strike");
        book.TryAddToNextSlot(skill1);

        // Create a different skill instance with the same name
        var skill2 = MockSkill.Create("Strike");

        book.Contains(skill2)
            .Should()
            .BeTrue();
    }
    #endregion

    #region AvailableSlots
    [Test]
    public void AvailableSlots_EmptyBook_ShouldReturnTotalSlots()
    {
        var book = new SkillBook();

        // 86 valid slots (89 - 3 invalid: 0, 36, 72)
        book.AvailableSlots
            .Should()
            .Be(86);
    }

    [Test]
    public void AvailableSlots_AfterAdding_ShouldDecrease()
    {
        var book = new SkillBook();
        book.TryAddToNextSlot(MockSkill.Create($"Skill{Guid.NewGuid():N}"));

        book.AvailableSlots
            .Should()
            .Be(85);
    }
    #endregion
}