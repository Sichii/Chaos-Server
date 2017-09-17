namespace Chaos
{
    using System.Collections.Generic;
    internal sealed class Dialog
    {
        internal DialogType Type { get; set; }
        internal ushort PursuitId { get; set; }
        internal ushort Id { get; set; }
        internal bool PrevBtn { get; set; }
        internal bool NextBtn { get; set; }
        internal string Message { get; set; }
        internal List<KeyValuePair<string, ushort>> Options { get; set; }
        internal ushort NextDialogId { get; set; }
        internal Panel<PanelObject> Panel { get; set; }
        internal ushort MaxCharacters { get; set; }

        /// <summary>
        /// Default constructor for dialog
        /// </summary>
        public Dialog(DialogType type, ushort pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, List<KeyValuePair<string,ushort>> options, ushort nextDialogId, Panel<PanelObject> panel, ushort maxCharacters)
        {
            Type = type;
            PursuitId = pursuitId;
            Id = dialogId;
            PrevBtn = prevBtn;
            NextBtn = nextBtn;
            Message = message;
            Options = options ?? new List<KeyValuePair<string, ushort>>();
            NextDialogId = nextDialogId;
            Panel = panel ?? new Panel<PanelObject>(0);
            MaxCharacters = maxCharacters;
        }

        /// <summary>
        /// Returns the next dialog in the sequence of dialogs.
        /// </summary>
        internal Dialog Next() => Game.Dialogs.NextDialog(NextDialogId);
        /// <summary>
        /// Returns the dialog associated with the option that was chosen.
        /// </summary>
        /// <param name="opt"></param>
        internal Dialog Next(byte opt) => Game.Dialogs.NextDialog(Options[opt - 1].Value);
        /// <summary>
        /// Returns the dialog that has this dialog as one of it's options or nextDialogId
        /// </summary>
        internal Dialog Previous() => Game.Dialogs.PreviousDialog(this);
        /// <summary>
        /// Returns an empty dialog with type DialogType.Close
        /// </summary>
        /// <returns></returns>
        internal static Dialog Close() => Game.Dialogs.CloseDialog();

    }
}
