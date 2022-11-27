using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Common.Definitions.MenuOrDialogType" />.
/// </summary>
public static class MenuOrDialogTypeExtensions
{
    /// <summary>
    ///     Converts a <see cref="Chaos.Common.Definitions.MenuOrDialogType" /> to a <see cref="Chaos.Common.Definitions.DialogType" />
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>
    ///     An equivalent <see cref="Chaos.Common.Definitions.DialogType" /> to the <see cref="Chaos.Common.Definitions.MenuOrDialogType" />
    ///     if one exists, otherwise <c>null</c>
    /// </returns>
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
    ///     Converts a <see cref="Chaos.Common.Definitions.MenuOrDialogType" /> to a <see cref="Chaos.Common.Definitions.MenuType" />
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>
    ///     An equivalent <see cref="Chaos.Common.Definitions.MenuType" /> to the <see cref="Chaos.Common.Definitions.MenuOrDialogType" /> if
    ///     one exists, otherwise <c>null</c>
    /// </returns>
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