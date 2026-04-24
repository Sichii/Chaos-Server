#region
using System.Collections.Concurrent;
using Chaos.Extensions.Common;
using Chaos.Scripting.Quests;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Services.Quests;

public sealed class QuestRegistry : IQuestRegistry
{
    private readonly ConcurrentDictionary<string, IReadOnlyList<DialogQuestHandler>> DialogIndex
        = new(StringComparer.OrdinalIgnoreCase);
    private readonly IServiceProvider Provider;
    private readonly ConcurrentDictionary<string, Quest> QuestsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<Type, Quest> QuestsByType = new();

    public QuestRegistry(IServiceProvider provider) => Provider = provider;

    /// <summary>
    /// Reflection-discovers all non-abstract Quest subclasses, instantiates and registers each.
    /// Called once during DI setup.
    /// </summary>
    public void AutoDiscover()
    {
        var questTypes = typeof(Quest).LoadImplementations()
                                      .Where(t => !t.IsAbstract);

        foreach (var type in questTypes)
        {
            var quest = (Quest)ActivatorUtilities.CreateInstance(Provider, type);
            quest.RunConfigure();
            Register(quest);
        }
    }

    /// <inheritdoc />
    public TQuest Get<TQuest>() where TQuest : Quest
    {
        if (!QuestsByType.TryGetValue(typeof(TQuest), out var quest))
            throw new KeyNotFoundException($"Quest of type {typeof(TQuest).Name} is not registered.");

        return (TQuest)quest;
    }

    /// <inheritdoc />
    public Quest? Get(string key) => QuestsByKey.GetValueOrDefault(key);

    /// <inheritdoc />
    public IReadOnlyList<DialogQuestHandler> GetDialogHandlers(string templateKey)
        => DialogIndex.GetValueOrDefault(templateKey) ?? [];

    /// <inheritdoc />
    public void Register(Quest quest)
    {
        if (!QuestsByKey.TryAdd(quest.Key, quest))
            throw new InvalidOperationException($"A quest with key '{quest.Key}' is already registered.");

        QuestsByType[quest.GetType()] = quest;

        foreach (var handler in quest.DialogHandlers)
            DialogIndex.AddOrUpdate(
                handler.TemplateKey,
                _ => new[] { handler },
                (_, existing) =>
                {
                    var next = new DialogQuestHandler[existing.Count + 1];

                    for (var i = 0; i < existing.Count; i++)
                        next[i] = existing[i];

                    next[existing.Count] = handler;

                    return next;
                });
    }
}
