using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.Dialog;

public sealed record Dialog
{
    public required string DialogKey { get; init; }
    public required string Text { get; init; }
    public required string? NextDialogKey { get; init; }
    public required string? PrevDialogKey { get; init; }
    public required MenuOrDialogType Type { get; init; }
    public required ICollection<DialogOption>? Options { get; init; }
    public required ICollection<Item>? Items { get; init; }
    public required ICollection<Spell>? Spells { get; init; }
    public required ICollection<Skill>? Skills { get; init; }
    public required ICollection<string>? ScriptKeys { get; init; }
    public required ushort? TextBoxLength { get; init; }

    public void TryActivate(Aisling aisling, object source)
    {
        if (!aisling.ActiveObject.TrySet(this))
            return;

        aisling.Client.SendDialog(this, source);
        //dialog.script.something idk
    }

    public static Dialog CloseDialog => new()
    {
        DialogKey = "close",
        Items = null,
        NextDialogKey = null,
        Options = null,
        PrevDialogKey = null,
        ScriptKeys = null,
        Skills = null,
        Spells = null,
        TextBoxLength = null,
        Text = string.Empty,
        Type = MenuOrDialogType.CloseDialog
    };
}