#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class EnumExtensionsTests
{
    [Test]
    public void GetFlags_ShouldReturnIndividualFlags()
    {
        // Arrange
        const SampleFlag1 COMBINED_FLAGS = SampleFlag1.Value1 | SampleFlag1.Value3;

        // Act
        var flags = COMBINED_FLAGS.GetFlags()
                                  .ToList();

        // Assert
        flags.Should()
             .Contain(
                 [
                     SampleFlag1.Value1,
                     SampleFlag1.Value3
                 ]);

        flags.Should()
             .NotContain([SampleFlag1.Value2]);
    }

    // ReSharper disable once ArrangeAttributes
    //@formatter:off
    [Test]
    [Arguments(EquipmentType.NotEquipment, new[]{ EquipmentSlot.None })]
    [Arguments(EquipmentType.Weapon, new[]{ EquipmentSlot.Weapon })]
    [Arguments(EquipmentType.Armor, new[]{ EquipmentSlot.Armor })]
    [Arguments(EquipmentType.OverArmor, new[]{ EquipmentSlot.Overcoat })]
    [Arguments(EquipmentType.Shield, new[]{ EquipmentSlot.Shield })]
    [Arguments(EquipmentType.Helmet, new[]{ EquipmentSlot.Helmet })]
    [Arguments(EquipmentType.OverHelmet, new[]{ EquipmentSlot.OverHelm })]
    [Arguments(EquipmentType.Earrings, new[]{ EquipmentSlot.Earrings })]
    [Arguments(EquipmentType.Necklace, new[]{ EquipmentSlot.Necklace })]
    [Arguments(EquipmentType.Ring, new[]{ EquipmentSlot.LeftRing, EquipmentSlot.RightRing })]
    [Arguments(EquipmentType.Gauntlet, new[]{ EquipmentSlot.LeftGaunt, EquipmentSlot.RightGaunt })]
    [Arguments(EquipmentType.Belt, new[]{ EquipmentSlot.Belt })]
    [Arguments(EquipmentType.Greaves, new[]{ EquipmentSlot.Greaves })]
    [Arguments(EquipmentType.Boots, new[]{ EquipmentSlot.Boots })]
    [Arguments(EquipmentType.Accessory, new[]{ 
        EquipmentSlot.Accessory1,
        EquipmentSlot.Accessory2,
        EquipmentSlot.Accessory3 })]
    //@formatter:on
    public void ToEquipmentSlots_Should_Return_Correct_EquipmentSlots_For_EquipmentType(
        EquipmentType equipmentType,
        IEnumerable<EquipmentSlot> expectedSlots)
    {
        // Act
        var equipmentSlots = equipmentType.ToEquipmentSlots();

        // Assert
        equipmentSlots.Should()
                      .BeEquivalentTo(expectedSlots);
    }

    [Test]
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