using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="MenuOrDialogType" />.
/// </summary>
public static class MenuOrDialogTypeExtensions
{
    /// <summary>
    ///     Converts a <see cref="MenuOrDialogType"/> to a <see cref="DialogType"/>
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>An equivalent <see cref="DialogType"/> to the <see cref="MenuOrDialogType"/> if one exists, otherwise <c>null</c></returns>
    public static DialogType? ToDialogType(this MenuOrDialogType value) => value switch
    {
        MenuOrDialogType.Normal          => DialogType.Normal,
        MenuOrDialogType.DialogMenu      => DialogType.DialogMenu,
        MenuOrDialogType.DialogTextEntry => DialogType.TextEntry,
        MenuOrDialogType.Speak           => DialogType.Speak,
        MenuOrDialogType.CreatureMenu    => DialogType.CreatureMenu,
        MenuOrDialogType.Protected       => DialogType.Protected,
        MenuOrDialogType.CloseDialog     => DialogType.CloseDialog,
        _                                => null
    };

    /// <summary>
    ///     Converts a <see cref="MenuOrDialogType"/> to a <see cref="MenuType"/>
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>An equivalent <see cref="MenuType"/> to the <see cref="MenuOrDialogType"/> if one exists, otherwise <c>null</c></returns>
    public static MenuType? ToMenuType(this MenuOrDialogType value) => value switch
    {
        MenuOrDialogType.Menu                  => MenuType.Menu,
        MenuOrDialogType.MenuWithArgs          => MenuType.MenuWithArgs,
        MenuOrDialogType.MenuTextEntry         => MenuType.TextEntry,
        MenuOrDialogType.MenuTextEntryWithArgs => MenuType.TextEntryWithArgs,
        MenuOrDialogType.ShowItems             => MenuType.ShowItems,
        MenuOrDialogType.ShowPlayerItems       => MenuType.ShowPlayerItems,
        MenuOrDialogType.ShowSpells            => MenuType.ShowSpells,
        MenuOrDialogType.ShowSkills            => MenuType.ShowSkills,
        MenuOrDialogType.ShowPlayerSpells      => MenuType.ShowPlayerSpells,
        MenuOrDialogType.ShowPlayerSkills      => MenuType.ShowPlayerSkills,
        _                                      => null
    };
}