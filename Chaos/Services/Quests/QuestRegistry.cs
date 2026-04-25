#region
using System.Collections.Concurrent;
using Chaos.Extensions.Common;
using Chaos.Utilities.QuestHelper;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Services.Quests;

public sealed class QuestRegistry : IQuestRegistry
{
    private readonly ConcurrentDictionary<DialogIndexKey, IReadOnlyList<DialogQuestHandler>> DialogIndex = new();
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
    public IReadOnlyList<DialogQuestHandler> GetDialogHandlers(string templateKey, DialogPhase phase)
        => DialogIndex.GetValueOrDefault(new DialogIndexKey(templateKey, phase)) ?? [];

    /// <inheritdoc />
    public void Register(Quest quest)
    {
        if (!QuestsByKey.TryAdd(quest.Key, quest))
            throw new InvalidOperationException($"A quest with key '{quest.Key}' is already registered.");

        QuestsByType[quest.GetType()] = quest;

        foreach (var handler in quest.DialogHandlers)
            DialogIndex.AddOrUpdate(
                new DialogIndexKey(handler.TemplateKey, handler.Phase),
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

    /// <summary>
    /// Composite index key. Custom record struct keeps the comparer case-insensitive on
    /// <see cref="TemplateKey" /> while leaving <see cref="Phase" /> untouched, mirroring the
    /// case-insensitive behavior the previous string-keyed dictionary provided.
    /// </summary>
    private readonly record struct DialogIndexKey(string TemplateKey, DialogPhase Phase)
    {
        public bool Equals(DialogIndexKey other)
            => (Phase == other.Phase) && string.Equals(TemplateKey, other.TemplateKey, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(TemplateKey), Phase);
    }
}
