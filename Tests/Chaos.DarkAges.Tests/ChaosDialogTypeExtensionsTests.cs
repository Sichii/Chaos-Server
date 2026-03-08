#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

namespace Chaos.DarkAges.Tests;

public sealed class ChaosDialogTypeExtensionsTests
{
    //@formatter:off
    [Test]
    [Arguments(ChaosDialogType.Normal,       DialogType.Normal)]
    [Arguments(ChaosDialogType.DialogMenu,   DialogType.DialogMenu)]
    [Arguments(ChaosDialogType.DialogTextEntry, DialogType.TextEntry)]
    [Arguments(ChaosDialogType.Speak,        DialogType.Speak)]
    [Arguments(ChaosDialogType.CreatureMenu, DialogType.CreatureMenu)]
    [Arguments(ChaosDialogType.Protected,    DialogType.Protected)]
    [Arguments(ChaosDialogType.CloseDialog,  DialogType.CloseDialog)]
    //@formatter:on
    public void ToDialogType_MappedValues_ReturnsExpected(ChaosDialogType input, DialogType expected)
        => input.ToDialogType()
                .Should()
                .Be(expected);

    [Test]
    public void ToDialogType_UnmappedValue_ReturnsNull()
        => ChaosDialogType.Menu
                          .ToDialogType()
                          .Should()
                          .BeNull();

    //@formatter:off
    [Test]
    [Arguments(ChaosDialogType.Menu,                  MenuType.Menu)]
    [Arguments(ChaosDialogType.MenuWithArgs,          MenuType.MenuWithArgs)]
    [Arguments(ChaosDialogType.MenuTextEntry,         MenuType.TextEntry)]
    [Arguments(ChaosDialogType.MenuTextEntryWithArgs, MenuType.TextEntryWithArgs)]
    [Arguments(ChaosDialogType.ShowItems,             MenuType.ShowItems)]
    [Arguments(ChaosDialogType.ShowPlayerItems,       MenuType.ShowPlayerItems)]
    [Arguments(ChaosDialogType.ShowSpells,            MenuType.ShowSpells)]
    [Arguments(ChaosDialogType.ShowSkills,            MenuType.ShowSkills)]
    [Arguments(ChaosDialogType.ShowPlayerSpells,      MenuType.ShowPlayerSpells)]
    [Arguments(ChaosDialogType.ShowPlayerSkills,      MenuType.ShowPlayerSkills)]
    //@formatter:on
    public void ToMenuType_MappedValues_ReturnsExpected(ChaosDialogType input, MenuType expected)
        => input.ToMenuType()
                .Should()
                .Be(expected);

    [Test]
    public void ToMenuType_UnmappedValue_ReturnsNull()
        => ChaosDialogType.Normal
                          .ToMenuType()
                          .Should()
                          .BeNull();
}