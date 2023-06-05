using Chaos.Common.Definitions;
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class EnumExtensionsTests
{
    // ReSharper disable once ArrangeAttributes
    [Theory]
    [InlineData(EquipmentType.NotEquipment, EquipmentSlot.None)]
    [InlineData(EquipmentType.Weapon, EquipmentSlot.Weapon)]
    [InlineData(EquipmentType.Armor, EquipmentSlot.Armor)]
    [InlineData(EquipmentType.OverArmor, EquipmentSlot.Overcoat)]
    [InlineData(EquipmentType.Shield, EquipmentSlot.Shield)]
    [InlineData(EquipmentType.Helmet, EquipmentSlot.Helmet)]
    [InlineData(EquipmentType.OverHelmet, EquipmentSlot.OverHelm)]
    [InlineData(EquipmentType.Earrings, EquipmentSlot.Earrings)]
    [InlineData(EquipmentType.Necklace, EquipmentSlot.Necklace)]
    [InlineData(EquipmentType.Ring, EquipmentSlot.LeftRing, EquipmentSlot.RightRing)]
    [InlineData(EquipmentType.Gauntlet, EquipmentSlot.LeftGaunt, EquipmentSlot.RightGaunt)]
    [InlineData(EquipmentType.Belt, EquipmentSlot.Belt)]
    [InlineData(EquipmentType.Greaves, EquipmentSlot.Greaves)]
    [InlineData(EquipmentType.Boots, EquipmentSlot.Boots)]
    [InlineData(
        EquipmentType.Accessory,
        EquipmentSlot.Accessory1,
        EquipmentSlot.Accessory2,
        EquipmentSlot.Accessory3)]
    public void ToEquipmentSlots_Should_Return_Correct_EquipmentSlots_For_EquipmentType(
        EquipmentType equipmentType,
        params EquipmentSlot[] expectedSlots
    )
    {
        // Act
        var equipmentSlots = equipmentType.ToEquipmentSlots();

        // Assert
        equipmentSlots.Should().BeEquivalentTo(expectedSlots);
    }

    [Fact]
    public void ToEquipmentSlots_Should_Throw_Exception_For_Undefined_EquipmentType()
    {
        // Arrange
        const EquipmentType EQUIPMENT_TYPE = (EquipmentType)99;

        // Act
        // ReSharper disable once IteratorMethodResultIsIgnored
        var func = () => EQUIPMENT_TYPE.ToEquipmentSlots();

        func.Enumerating()
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithParameterName("type");
    }
}