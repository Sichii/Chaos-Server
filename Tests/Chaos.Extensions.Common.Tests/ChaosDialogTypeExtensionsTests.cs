#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class ChaosDialogTypeExtensionsTests
{
    [Test]
    [Arguments(ChaosDialogType.Normal, DialogType.Normal)]
    [Arguments(ChaosDialogType.DialogMenu, DialogType.DialogMenu)]
    [Arguments(ChaosDialogType.DialogTextEntry, DialogType.TextEntry)]
    [Arguments(ChaosDialogType.Speak, DialogType.Speak)]
    [Arguments(ChaosDialogType.CreatureMenu, DialogType.CreatureMenu)]
    [Arguments(ChaosDialogType.Protected, DialogType.Protected)]
    [Arguments(ChaosDialogType.CloseDialog, DialogType.CloseDialog)]
    public void ToDialogType_Should_Convert_Correctly(ChaosDialogType chaosDialogType, DialogType? expectedDialogType)
    {
        // Act
        var dialogType = chaosDialogType.ToDialogType();

        // Assert
        dialogType.Should()
                  .Be(expectedDialogType);
    }

    [Test]
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

    [Test]
    [Arguments(ChaosDialogType.Menu, MenuType.Menu)]
    [Arguments(ChaosDialogType.MenuWithArgs, MenuType.MenuWithArgs)]
    [Arguments(ChaosDialogType.MenuTextEntry, MenuType.TextEntry)]
    [Arguments(ChaosDialogType.MenuTextEntryWithArgs, MenuType.TextEntryWithArgs)]
    [Arguments(ChaosDialogType.ShowItems, MenuType.ShowItems)]
    [Arguments(ChaosDialogType.ShowPlayerItems, MenuType.ShowPlayerItems)]
    [Arguments(ChaosDialogType.ShowSpells, MenuType.ShowSpells)]
    [Arguments(ChaosDialogType.ShowSkills, MenuType.ShowSkills)]
    [Arguments(ChaosDialogType.ShowPlayerSpells, MenuType.ShowPlayerSpells)]
    [Arguments(ChaosDialogType.ShowPlayerSkills, MenuType.ShowPlayerSkills)]
    public void ToMenuType_Should_Convert_Correctly(ChaosDialogType chaosDialogType, MenuType? expectedMenuType)
    {
        // Act
        var menuType = chaosDialogType.ToMenuType();

        // Assert
        menuType.Should()
                .Be(expectedMenuType);
    }

    [Test]
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