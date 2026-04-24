namespace Chaos.Scripting.Quests;

/// <summary>
/// Records one quest's reaction to a specific dialog template key.
/// Operations are evaluated in order; each returns true to continue the chain,
/// false to halt (e.g., a guard failed).
/// </summary>
public sealed record DialogQuestHandler(
    Quest Quest,
    string TemplateKey,
    IReadOnlyList<Func<QuestContext, bool>> Operations);
