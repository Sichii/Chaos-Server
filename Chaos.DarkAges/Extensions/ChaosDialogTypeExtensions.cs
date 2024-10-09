using Chaos.DarkAges.Definitions;

namespace Chaos.DarkAges.Extensions;

/// <summary>
///     Provides extension methods for <see cref="ChaosDialogType" />.
/// </summary>
public static class ChaosDialogTypeExtensions
{
    /// <summary>
    ///     Converts a <see cref="ChaosDialogType" /> to a <see cref="value" />
    /// </summary>
    /// <param name="value">
    ///     The value to convert
    /// </param>
    /// <returns>
    ///     An equivalent <see cref="DialogType" /> to the <see cref="DialogType" /> if one exists, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </returns>
    public static DialogType? ToDialogType(this ChaosDialogType value)
        => value switch
        {
            ChaosDialogType.Normal          => DialogType.Normal,
            ChaosDialogType.DialogMenu      => DialogType.DialogMenu,
            ChaosDialogType.DialogTextEntry => DialogType.TextEntry,
            ChaosDialogType.Speak           => DialogType.Speak,
            ChaosDialogType.CreatureMenu    => DialogType.CreatureMenu,
            ChaosDialogType.Protected       => DialogType.Protected,
            ChaosDialogType.CloseDialog     => DialogType.CloseDialog,
            _                               => null
        };

    /// <summary>
    ///     Converts a <see cref="ChaosDialogType" /> to a <see cref="value" />
    /// </summary>
    /// <param name="value">
    ///     The value to convert
    /// </param>
    /// <returns>
    ///     An equivalent <see cref="MenuType" /> to the <see cref="MenuType" /> if one exists, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </returns>
    public static MenuType? ToMenuType(this ChaosDialogType value)
        => value switch
        {
            ChaosDialogType.Menu                  => MenuType.Menu,
            ChaosDialogType.MenuWithArgs          => MenuType.MenuWithArgs,
            ChaosDialogType.MenuTextEntry         => MenuType.TextEntry,
            ChaosDialogType.MenuTextEntryWithArgs => MenuType.TextEntryWithArgs,
            ChaosDialogType.ShowItems             => MenuType.ShowItems,
            ChaosDialogType.ShowPlayerItems       => MenuType.ShowPlayerItems,
            ChaosDialogType.ShowSpells            => MenuType.ShowSpells,
            ChaosDialogType.ShowSkills            => MenuType.ShowSkills,
            ChaosDialogType.ShowPlayerSpells      => MenuType.ShowPlayerSpells,
            ChaosDialogType.ShowPlayerSkills      => MenuType.ShowPlayerSkills,
            _                                     => null
        };
}