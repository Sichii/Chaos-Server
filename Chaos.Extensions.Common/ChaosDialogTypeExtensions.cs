using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="ChaosDialogType" />.
/// </summary>
public static class ChaosDialogTypeExtensions
{
    /// <summary>
    ///     Converts a <see cref="ChaosDialogType" /> to a <see cref="Chaos.Common.Definitions.DialogType" />
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>
    ///     An equivalent <see cref="Chaos.Common.Definitions.DialogType" /> to the <see cref="ChaosDialogType" />
    ///     if one exists, otherwise <c>null</c>
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
    ///     Converts a <see cref="ChaosDialogType" /> to a <see cref="Chaos.Common.Definitions.MenuType" />
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>
    ///     An equivalent <see cref="Chaos.Common.Definitions.MenuType" /> to the <see cref="ChaosDialogType" /> if
    ///     one exists, otherwise <c>null</c>
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