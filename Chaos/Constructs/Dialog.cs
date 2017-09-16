using System.Collections.Generic;

namespace Chaos
{
    using System.Linq;
    using System.Collections.Generic;
    internal sealed class Dialog
    {
        internal MenuType DialogType { get; set; }
        internal ushort PursuitId { get; set; }
        internal ushort Id { get; set; }
        internal bool PrevBtn { get; set; }
        internal bool NextBtn { get; set; }
        internal string Message { get; set; }
        internal SortedDictionary<string, ushort> Options { get; set; }
        internal ushort NextDialogId { get; set; }
        internal Panel<PanelObject> Panel { get; set; }

        /// <summary>
        /// Creates a standard dialog with or without buttons
        /// </summary>
        /// <param name="nextDialogId">The dialog that comes after this one if you press next.</param>
        internal Dialog(MenuType dialogType, ushort pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, ushort nextDialogId = 0)
        {
            DialogType = dialogType;
            PursuitId = pursuitId;
            Id = dialogId;
            PrevBtn = prevBtn;
            NextBtn = nextBtn;
            Message = message;
            NextDialogId = nextDialogId;
        }

        /// <summary>
        /// Creates a dialog that has a menu on it that leads to other dialogs or effects
        /// </summary>
        /// <param name="options">List of dialog options, and the dialog id they each lead to.</param>
        internal Dialog(MenuType dialogType, ushort pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, SortedDictionary<string, ushort> options)
            : this(dialogType, pursuitId, dialogId, prevBtn, nextBtn, message)
        {
            Options = options;
        }

        /// <summary>
        /// Creates a dialog that has a buy/sell menu, or learn/forget skill/spell menu
        /// </summary>
        /// <param name="panel">The panel you want to display.</param>
        internal Dialog(MenuType dialogType, ushort pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, Panel<PanelObject> panel)
            : this(dialogType, pursuitId, dialogId, prevBtn, nextBtn, message)
        {
            Panel = panel;
        }

        internal Dialog Next() => Game.Dialogs.NextDialog(Id);
        internal Dialog Next(byte opt) => Game.Dialogs.NextDialog(Options.Values.ElementAt(opt));
        internal Dialog Previous() => Game.Dialogs.PreviousDialog(this);
    }
}
