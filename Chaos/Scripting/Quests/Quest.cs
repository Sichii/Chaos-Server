#region
using Chaos.Models.Menu;
using Chaos.Models.World;
#endregion

namespace Chaos.Scripting.Quests;

/// <summary>
/// Non-generic base — what the registry stores and what dispatchers reference.
/// </summary>
public abstract class Quest
{
    public abstract string Key { get; }

    /// <summary>
    /// Internal collection of dialog handlers populated when Configure runs.
    /// Set by Quest&lt;TStage&gt;.RunConfigure.
    /// </summary>
    internal IReadOnlyList<DialogQuestHandler> DialogHandlers { get; set; } = [];

    internal abstract void RunConfigure();

    /// <summary>
    /// Build a strongly-typed <see cref="QuestContext" /> for this quest. Dispatchers call this to
    /// obtain a context typed to the quest's <c>TStage</c> without knowing the type at compile time.
    /// </summary>
    internal abstract QuestContext CreateContextFor(Aisling source, Dialog? subject, IServiceProvider services);
}

/// <summary>
/// Strongly-typed quest base. Each quest has a primary stage enum stored in Trackers.Enums.
/// </summary>
public abstract class Quest<TStage> : Quest where TStage : struct, Enum
{
    protected abstract void Configure(QuestBuilder<TStage> q);

    internal sealed override void RunConfigure()
    {
        var builder = new QuestBuilder<TStage>(this);
        Configure(builder);
        DialogHandlers = builder.BuildDialogHandlers();
    }

    internal sealed override QuestContext CreateContextFor(Aisling source, Dialog? subject, IServiceProvider services)
    {
        // Read current stage from Trackers; default if absent.
        source.Trackers.Enums.TryGetValue<TStage>(out var stage);

        return new QuestContext<TStage>
        {
            Source = source,
            Subject = subject,
            CurrentStage = stage,
            Services = services
        };
    }
}
