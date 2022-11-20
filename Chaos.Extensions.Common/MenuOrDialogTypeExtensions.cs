using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

public static class MenuOrDialogTypeExtensions
{
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