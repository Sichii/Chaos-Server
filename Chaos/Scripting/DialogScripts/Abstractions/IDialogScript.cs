using Chaos.Models.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.DialogScripts.Abstractions;

public interface IDialogScript : IScript
{
    /// <summary>
    ///     Called when the dialog has been displayed
    /// </summary>
    /// <param name="source">The aisling the dialog is for</param>
    void OnDisplayed(Aisling source);

    /// <summary>
    ///     Called when the dialog is about to be displayed, but has not yet been displayed. You can use this event to modify
    ///     the dialog before it
    ///     is displayed, such as injecting text parameters into the dialog's text
    /// </summary>
    /// <param name="source">The aisling the dialog is for</param>
    void OnDisplaying(Aisling source);

    /// <summary>
    ///     Called when the player responds to the dialog, whether by clicking Next, selecting an option, or making any kind of
    ///     selection<br />
    ///     If an option is clicked, the optionIndex will be the index of the option that was clicked<br />
    ///     If a shop selection was clicked, the text of that option will be in the MenuArgs property of the dialog<br />
    ///     If a selection was made from the player's inventory, spellbook, or skillbook, the Slot that was selected will be in
    ///     the MenuArgs
    ///     property of the dialog
    /// </summary>
    /// <param name="source">The aisling the dialog is for</param>
    /// <param name="optionIndex">The index of the option that was selected, if any</param>
    void OnNext(Aisling source, byte? optionIndex = null);

    /// <summary>
    ///     Called when the player clicks the Previous button on a Normal dialog
    /// </summary>
    /// <param name="source">The aisling the dialog is for</param>
    void OnPrevious(Aisling source);
}