#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class EquipmentTests
{
    private static Item CreateEquipItem(string name = "TestEquip", EquipmentType? equipmentType = null)
        => MockItem.Create(
            name,
            setup: item =>
            {
                // EquipmentType is init-only on the template, so we use reflection to set it for testing
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(item.Template, equipmentType);
            });

    #region Constructor
    [Test]
    public void Constructor_Default_ShouldBeEmpty()
    {
        var equipment = new Equipment();

        equipment.Count
                 .Should()
                 .Be(0);
    }

    [Test]
    public void Constructor_WithItems_ShouldPopulate()
    {
        var item = CreateEquipItem("Sword", EquipmentType.Weapon);
        item.Slot = (byte)EquipmentSlot.Weapon;

        var equipment = new Equipment([item]);

        equipment.Count
                 .Should()
                 .Be(1);

        equipment[EquipmentSlot.Weapon]
            .Should()
            .NotBeNull();
    }
    #endregion

    #region TryEquip
    [Test]
    public void TryEquip_ShouldSucceed_WhenSlotEmpty()
    {
        var equipment = new Equipment();
        var item = CreateEquipItem("Sword", EquipmentType.Weapon);

        var result = equipment.TryEquip(EquipmentType.Weapon, item, out var returned);

        result.Should()
              .BeTrue();

        returned.Should()
                .BeNull();

        equipment[EquipmentSlot.Weapon]
            .Should()
            .BeSameAs(item);
    }

    [Test]
    public void TryEquip_NotEquipment_ShouldThrowInvalidOperationException()
    {
        var equipment = new Equipment();
        var item = CreateEquipItem("Junk");

        var act = () => equipment.TryEquip(EquipmentType.NotEquipment, item, out _);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void TryEquip_Ring_ShouldFillLeftRingFirst()
    {
        var equipment = new Equipment();
        var ring1 = CreateEquipItem("Ring1", EquipmentType.Ring);

        equipment.TryEquip(EquipmentType.Ring, ring1, out _);

        equipment[EquipmentSlot.LeftRing]
            .Should()
            .BeSameAs(ring1);

        equipment[EquipmentSlot.RightRing]
            .Should()
            .BeNull();
    }

    [Test]
    public void TryEquip_Ring_ShouldFillRightRing_WhenLeftOccupied()
    {
        var equipment = new Equipment();
        var ring1 = CreateEquipItem("Ring1", EquipmentType.Ring);
        var ring2 = CreateEquipItem("Ring2", EquipmentType.Ring);

        equipment.TryEquip(EquipmentType.Ring, ring1, out _);
        equipment.TryEquip(EquipmentType.Ring, ring2, out _);

        equipment[EquipmentSlot.LeftRing]
            .Should()
            .BeSameAs(ring1);

        equipment[EquipmentSlot.RightRing]
            .Should()
            .BeSameAs(ring2);
    }

    [Test]
    public void TryEquip_Ring_ShouldReplaceLastSlot_WhenBothOccupied()
    {
        var equipment = new Equipment();
        var ring1 = CreateEquipItem("Ring1", EquipmentType.Ring);
        var ring2 = CreateEquipItem("Ring2", EquipmentType.Ring);
        var ring3 = CreateEquipItem("Ring3", EquipmentType.Ring);

        equipment.TryEquip(EquipmentType.Ring, ring1, out _);
        equipment.TryEquip(EquipmentType.Ring, ring2, out _);
        equipment.TryEquip(EquipmentType.Ring, ring3, out var returned);

        returned.Should()
                .BeSameAs(ring2);

        equipment[EquipmentSlot.RightRing]
            .Should()
            .BeSameAs(ring3);
    }

    [Test]
    public void TryEquip_SingleSlot_ShouldReturnOldItem_WhenOccupied()
    {
        var equipment = new Equipment();
        var sword1 = CreateEquipItem("Sword1", EquipmentType.Weapon);
        var sword2 = CreateEquipItem("Sword2", EquipmentType.Weapon);

        equipment.TryEquip(EquipmentType.Weapon, sword1, out _);
        equipment.TryEquip(EquipmentType.Weapon, sword2, out var returned);

        returned.Should()
                .BeSameAs(sword1);

        equipment[EquipmentSlot.Weapon]
            .Should()
            .BeSameAs(sword2);
    }
    #endregion

    #region Indexer / Contains / Remove
    [Test]
    public void Indexer_ByEquipmentSlot_ShouldReturnNull_WhenEmpty()
    {
        var equipment = new Equipment();

        equipment[EquipmentSlot.Weapon]
            .Should()
            .BeNull();
    }

    [Test]
    public void Contains_BySlot_ShouldReturnTrue_WhenOccupied()
    {
        var equipment = new Equipment();
        var item = CreateEquipItem("Helm", EquipmentType.Helmet);
        equipment.TryEquip(EquipmentType.Helmet, item, out _);

        equipment.Contains((byte)EquipmentSlot.Helmet)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void Remove_ShouldRemoveItem()
    {
        var equipment = new Equipment();
        var item = CreateEquipItem("Helm", EquipmentType.Helmet);
        equipment.TryEquip(EquipmentType.Helmet, item, out _);

        equipment.Remove((byte)EquipmentSlot.Helmet)
                 .Should()
                 .BeTrue();

        equipment[EquipmentSlot.Helmet]
            .Should()
            .BeNull();
    }

    [Test]
    public void Count_ShouldReflectEquippedItems()
    {
        var equipment = new Equipment();
        var weapon = CreateEquipItem("Sword", EquipmentType.Weapon);
        var armor = CreateEquipItem("Plate", EquipmentType.Armor);

        equipment.TryEquip(EquipmentType.Weapon, weapon, out _);
        equipment.TryEquip(EquipmentType.Armor, armor, out _);

        equipment.Count
                 .Should()
                 .Be(2);
    }
    #endregion
}