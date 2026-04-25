namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Records one quest's reaction to a specific dialog template key on a specific lifecycle
/// phase. Operations are evaluated in order; each returns true to continue the chain,
/// false to halt (e.g., a guard failed).
/// </summary>
public sealed record DialogQuestHandler(
    Quest Quest,
    string TemplateKey,
    DialogPhase Phase,
    IReadOnlyList<Func<QuestContext, bool>> Operations);
