namespace Chaos.Scripting.Quests;

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
    /// Register a handler chain for a dialog template key. The returned QuestStepBuilder
    /// records the operation chain via further fluent calls.
    /// </summary>
    public QuestStepBuilder<TStage> OnDialog(string templateKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateKey);

        var step = new QuestStepBuilder<TStage>();
        DialogHandlers.Add(new DialogQuestHandler(Quest, templateKey, step.Build()));

        return step;
    }
}
