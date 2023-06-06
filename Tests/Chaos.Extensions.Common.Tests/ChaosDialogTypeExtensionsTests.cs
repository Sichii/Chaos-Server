using Chaos.Common.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class ChaosDialogTypeExtensionsTests
{
    [Theory]
    [InlineData(ChaosDialogType.Normal, DialogType.Normal)]
    [InlineData(ChaosDialogType.DialogMenu, DialogType.DialogMenu)]
    [InlineData(ChaosDialogType.DialogTextEntry, DialogType.TextEntry)]
    [InlineData(ChaosDialogType.Speak, DialogType.Speak)]
    [InlineData(ChaosDialogType.CreatureMenu, DialogType.CreatureMenu)]
    [InlineData(ChaosDialogType.Protected, DialogType.Protected)]
    [InlineData(ChaosDialogType.CloseDialog, DialogType.CloseDialog)]
    public void ToDialogType_Should_Convert_Correctly(ChaosDialogType chaosDialogType, DialogType? expectedDialogType)
    {
        // Act
        var dialogType = chaosDialogType.ToDialogType();

        // Assert
        dialogType.Should().Be(expectedDialogType);
    }

    [Fact]
    public void ToDialogType_Should_ReturnNull_For_Undefined_ChaosDialogType()
    {
        // Arrange
        const ChaosDialogType CHAOS_DIALOG_TYPE = (ChaosDialogType)99;

        // Act
        // ReSharper disable once IteratorMethodResultIsIgnored
        var func = () => CHAOS_DIALOG_TYPE.ToDialogType();

        func.Should()
            .NotThrow()
            .Which
            .Should()
            .BeNull();
    }

    [Theory]
    [InlineData(ChaosDialogType.Menu, MenuType.Menu)]
    [InlineData(ChaosDialogType.MenuWithArgs, MenuType.MenuWithArgs)]
    [InlineData(ChaosDialogType.MenuTextEntry, MenuType.TextEntry)]
    [InlineData(ChaosDialogType.MenuTextEntryWithArgs, MenuType.TextEntryWithArgs)]
    [InlineData(ChaosDialogType.ShowItems, MenuType.ShowItems)]
    [InlineData(ChaosDialogType.ShowPlayerItems, MenuType.ShowPlayerItems)]
    [InlineData(ChaosDialogType.ShowSpells, MenuType.ShowSpells)]
    [InlineData(ChaosDialogType.ShowSkills, MenuType.ShowSkills)]
    [InlineData(ChaosDialogType.ShowPlayerSpells, MenuType.ShowPlayerSpells)]
    [InlineData(ChaosDialogType.ShowPlayerSkills, MenuType.ShowPlayerSkills)]
    public void ToMenuType_Should_Convert_Correctly(ChaosDialogType chaosDialogType, MenuType? expectedMenuType)
    {
        // Act
        var menuType = chaosDialogType.ToMenuType();

        // Assert
        menuType.Should().Be(expectedMenuType);
    }

    [Fact]
    public void ToMenuType_Should_ReturnNull_For_Undefined_ChaosDialogType()
    {
        // Arrange
        const ChaosDialogType CHAOS_DIALOG_TYPE = (ChaosDialogType)99;

        // Act
        // ReSharper disable once IteratorMethodResultIsIgnored
        var func = () => CHAOS_DIALOG_TYPE.ToMenuType();

        func.Should()
            .NotThrow()
            .Which
            .Should()
            .BeNull();
    }
}