namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Identifies which dialog lifecycle event a quest handler is bound to. Mirrors
/// <see cref="Chaos.Scripting.DialogScripts.Abstractions.IDialogScript" />'s four overrides.
/// Quests register handlers per phase via <c>QuestBuilder.OnDisplaying</c> /
/// <c>OnDisplayed</c> / <c>OnNext</c> / <c>OnPrevious</c>.
/// </summary>
public enum DialogPhase
{
    /// <summary>
    /// Dialog is about to be sent to the client. Use this phase for read-only setup
    /// (option injection, text templating). Mutating quest state here will fire before
    /// the player sees the dialog and is almost always wrong.
    /// </summary>
    Displaying,

    /// <summary>
    /// Dialog has been sent. Rarely the right phase for quest work — kept for parity
    /// with <c>IDialogScript.OnDisplayed</c>.
    /// </summary>
    Displayed,

    /// <summary>
    /// Player advanced the dialog (Next button, option click, menu selection). The
    /// commit phase: Advance, GiveItem, ConsumeItem, GiveExperience, etc. belong here.
    /// </summary>
    Next,

    /// <summary>
    /// Player backed up via the Previous button on a Normal dialog.
    /// </summary>
    Previous
}
