#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

namespace Chaos.DarkAges.Tests;

public sealed class EnumExtensionsTests
{
    [Test]
    public void ToEquipmentSlots_Accessory_ReturnsAllThreeAccessorySlots()
        => EquipmentType.Accessory
                        .ToEquipmentSlots()
                        .Should()
                        .Equal(EquipmentSlot.Accessory1, EquipmentSlot.Accessory2, EquipmentSlot.Accessory3);

    [Test]
    public void ToEquipmentSlots_Gauntlet_ReturnsBothGauntSlots()
        => EquipmentType.Gauntlet
                        .ToEquipmentSlots()
                        .Should()
                        .Equal(EquipmentSlot.LeftGaunt, EquipmentSlot.RightGaunt);

    [Test]
    public void ToEquipmentSlots_Ring_ReturnsBothRingSlots()
        => EquipmentType.Ring
                        .ToEquipmentSlots()
                        .Should()
                        .Equal(EquipmentSlot.LeftRing, EquipmentSlot.RightRing);

    //@formatter:off
    [Test]
    [Arguments(EquipmentType.NotEquipment, new[] { EquipmentSlot.None })]
    [Arguments(EquipmentType.Weapon,       new[] { EquipmentSlot.Weapon })]
    [Arguments(EquipmentType.Armor,        new[] { EquipmentSlot.Armor })]
    [Arguments(EquipmentType.OverArmor,    new[] { EquipmentSlot.Overcoat })]
    [Arguments(EquipmentType.Shield,       new[] { EquipmentSlot.Shield })]
    [Arguments(EquipmentType.Helmet,       new[] { EquipmentSlot.Helmet })]
    [Arguments(EquipmentType.OverHelmet,   new[] { EquipmentSlot.OverHelm })]
    [Arguments(EquipmentType.Earrings,     new[] { EquipmentSlot.Earrings })]
    [Arguments(EquipmentType.Necklace,     new[] { EquipmentSlot.Necklace })]
    [Arguments(EquipmentType.Belt,         new[] { EquipmentSlot.Belt })]
    [Arguments(EquipmentType.Greaves,      new[] { EquipmentSlot.Greaves })]
    [Arguments(EquipmentType.Boots,        new[] { EquipmentSlot.Boots })]
    //@formatter:on
    public void ToEquipmentSlots_SingleSlotCases_ReturnsSingleSlot(EquipmentType type, EquipmentSlot[] expected)
        => type.ToEquipmentSlots()
               .Should()
               .Equal(expected);

    [Test]
    public void ToEquipmentSlots_UndefinedValue_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ((EquipmentType)255).ToEquipmentSlots()
                                            .ToList();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
}