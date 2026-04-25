namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Fluent root passed to Quest&lt;TStage&gt;.Configure. Collects DialogQuestHandlers
/// (and, in v2, monster/item/map/reactor handlers).
/// </summary>
public sealed class QuestBuilder<TStage> where TStage : struct, Enum
{
    private readonly Quest Quest;
    private readonly List<DialogQuestHandler> DialogHandlers = [];

    internal QuestBuilder(Quest quest) => Quest = quest;

    internal IReadOnlyList<DialogQuestHandler> BuildDialogHandlers() => DialogHandlers;

    /// <summary>
    /// Register a handler chain that fires when a dialog matching <paramref name="templateKey" />
    /// is about to be sent to the player. Use this phase for option injection and text templating;
    /// mutating quest state here fires before the player sees the dialog.
    /// </summary>
    public QuestStepBuilder<TStage> OnDisplaying(string templateKey)
        => RegisterHandler(templateKey, DialogPhase.Displaying);

    /// <summary>
    /// Register a handler chain that fires after a dialog matching <paramref name="templateKey" />
    /// has been sent to the player. Rarely the right phase — kept for parity with
    /// <c>IDialogScript.OnDisplayed</c>.
    /// </summary>
    public QuestStepBuilder<TStage> OnDisplayed(string templateKey)
        => RegisterHandler(templateKey, DialogPhase.Displayed);

    /// <summary>
    /// Register a handler chain that fires when the player advances a dialog matching
    /// <paramref name="templateKey" /> (Next button, option click, menu selection). This is the
    /// commit phase — Advance, GiveItem, ConsumeItem, GiveExperience, and other state mutations
    /// belong here.
    /// </summary>
    public QuestStepBuilder<TStage> OnNext(string templateKey)
        => RegisterHandler(templateKey, DialogPhase.Next);

    /// <summary>
    /// Register a handler chain that fires when the player backs up via the Previous button on a
    /// Normal dialog matching <paramref name="templateKey" />.
    /// </summary>
    public QuestStepBuilder<TStage> OnPrevious(string templateKey)
        => RegisterHandler(templateKey, DialogPhase.Previous);

    private QuestStepBuilder<TStage> RegisterHandler(string templateKey, DialogPhase phase)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateKey);

        var step = new QuestStepBuilder<TStage>();
        DialogHandlers.Add(new DialogQuestHandler(Quest, templateKey, phase, step.Build()));

        return step;
    }

}
