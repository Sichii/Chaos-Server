// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

namespace Chaos
{
    internal sealed class Dialog
    {
        internal DialogType Type { get; }
        internal PursuitIds PursuitId { get; }
        internal ushort Id { get; }
        internal bool PrevBtn { get; }
        internal bool NextBtn { get; }
        internal string Message { get; }
        internal DialogMenu Menu { get; }
        internal ushort NextDialogId { get; }
        internal ushort PreviousDialogId { get; set; }
        internal Panel<PanelObject> Panel { get; }
        internal ushort MaxCharacters { get; }

        /// <summary>
        /// Default constructor for dialog
        /// </summary>
        public Dialog(DialogType type, PursuitIds pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, DialogMenu menu, ushort nextDialogId, Panel<PanelObject> panel, ushort maxCharacters)
        {
            Type = type;
            PursuitId = pursuitId;
            Id = dialogId;
            PrevBtn = prevBtn;
            NextBtn = nextBtn;
            Message = message;
            Menu = menu ?? null;
            NextDialogId = nextDialogId;
            Panel = panel ?? null;
            MaxCharacters = maxCharacters;
        }

        /// <summary>
        /// Returns the next dialog in the sequence of dialogs.
        /// </summary>
        internal Dialog Next() => Game.Dialogs[NextDialogId];
        /// <summary>
        /// Returns the dialog associated with the option that was chosen.
        /// </summary>
        /// <param name="opt"></param>
        internal Dialog Next(byte opt) => Game.Dialogs[Menu[opt].DialogId];
        /// <summary>
        /// Returns the dialog that has this dialog as one of it's options or nextDialogId
        /// </summary>
        internal Dialog Previous() => Game.Dialogs[PreviousDialogId];
        /// <summary>
        /// Returns an empty dialog with type DialogType.Close
        /// </summary>
        /// <returns></returns>
        internal Dialog Close(bool applyEffect = false) => new Dialog(DialogType.CloseDialog, applyEffect ? PursuitId : PursuitIds.None, 0, false, false, string.Empty, null, 0, null, 0);

    }
}
