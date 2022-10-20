using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

public static class EnumExtensions
{
    public static DialogType? ToDialogType(this MenuOrDialogType value) => value switch
    {
        MenuOrDialogType.Normal          => DialogType.Normal,
        MenuOrDialogType.ItemMenu        => DialogType.ItemMenu,
        MenuOrDialogType.DialogTextEntry => DialogType.TextEntry,
        MenuOrDialogType.Speak           => DialogType.Speak,
        MenuOrDialogType.CreatureMenu    => DialogType.CreatureMenu,
        MenuOrDialogType.Protected       => DialogType.Protected,
        MenuOrDialogType.CloseDialog     => DialogType.CloseDialog,
        _                                => null
    };

    public static MenuType? ToMenuType(this MenuOrDialogType value) => value switch
    {
        MenuOrDialogType.Menu              => MenuType.Menu,
        MenuOrDialogType.MenuWithArgs      => MenuType.MenuWithArgs,
        MenuOrDialogType.MenuTextEntry     => MenuType.TextEntry,
        MenuOrDialogType.ShowItems         => MenuType.ShowItems,
        MenuOrDialogType.ShowOwnedItems    => MenuType.ShowOwnedItems,
        MenuOrDialogType.ShowSpells        => MenuType.ShowSpells,
        MenuOrDialogType.ShowSkills        => MenuType.ShowSkills,
        MenuOrDialogType.ShowLearnedSpells => MenuType.ShowLearnedSpells,
        MenuOrDialogType.ShowLearnedSkills => MenuType.ShowLearnedSkills,
        _                                  => null
    };
}