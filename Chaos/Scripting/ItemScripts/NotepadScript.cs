#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
#endregion

namespace Chaos.Scripting.ItemScripts;

public class NotepadScript : ItemScriptBase
{
    /// <inheritdoc />
    public NotepadScript(Item subject)
        : base(subject) { }

    //in da, vday note has 2 sprites, 143 and 216
    //216 is a note with no text, 143 is a note with text
    //so we update the panelsprite depending on if the notepad has text or not
    /// <inheritdoc />
    public override void OnNotepadTextUpdated(Aisling source, string? oldText)
        => source.Inventory.Update(
            Subject.Slot,
            localItem => localItem.ItemSprite.PanelSprite = string.IsNullOrEmpty(Subject.NotepadText) ? (ushort)216 : (ushort)143);

    /// <inheritdoc />
    public override void OnUse(Aisling source)
        => source.Client.SendDisplayNotepad(
            NotepadType.White,
            Subject,
            30,
            20);
}