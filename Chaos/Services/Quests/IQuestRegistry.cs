#region
using Chaos.Utilities.QuestHelper;
#endregion

namespace Chaos.Services.Quests;

public interface IQuestRegistry
{
    /// <summary>
    /// Get a registered quest by its concrete type.
    /// </summary>
    TQuest Get<TQuest>() where TQuest : Quest;

    /// <summary>
    /// Get a registered quest by its string key.
    /// </summary>
    Quest? Get(string key);

    /// <summary>
    /// All dialog handlers (across all quests) registered for a given dialog template key.
    /// Returns an empty list if no quest handles this template.
    /// </summary>
    IReadOnlyList<DialogQuestHandler> GetDialogHandlers(string templateKey);

    /// <summary>
    /// Register a quest. Runs Configure and indexes its handlers. Throws if a quest with
    /// the same Key is already registered.
    /// </summary>
    void Register(Quest quest);
}
