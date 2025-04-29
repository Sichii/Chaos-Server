#region
using Chaos.Models.World.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ScriptedSequence<T> : IDeltaUpdatable where T: Creature
{
    private readonly List<ConditionalAction<T>> ConditionalActions = [];
    private readonly T Entity;
    private readonly List<ConditionalAction<T>> OneTimeConditionalActions = [];
    private readonly List<TimedAction<T>> RepeatedTimedActions = [];
    private readonly List<TimedActionSequence<T>> RepeatedTimedActionSequences = [];
    private readonly IIntervalTimer ScriptTimer;
    private readonly TimeSpan ScriptUpdateInterval;
    private readonly List<ThresholdAction<T>> ThresholdActions = [];
    private readonly List<TimedAction<T>> TimedActions = [];
    private readonly List<TimedActionSequence<T>> TimedActionSequences = [];

    public ScriptedSequence(
        T entity,
        TimeSpan scriptUpdateInterval,
        List<ThresholdActionDescriptor<T>> thresholdActions,
        List<TimedActionDescriptor<T>> timedActions,
        List<TimedActionDescriptor<T>> repeatedTimedActions,
        List<TimedActionSequenceDescriptor<T>> timedActionSequences,
        List<TimedActionSequenceDescriptor<T>> repeatedTimedActionSequences,
        List<ConditionalActionDescriptor<T>> conditionalActions,
        List<ConditionalActionDescriptor<T>> oneTimeConditionalActions)
    {
        Entity = entity;
        ScriptUpdateInterval = scriptUpdateInterval;
        ScriptTimer = new IntervalTimer(scriptUpdateInterval);

        foreach (var action in thresholdActions)
            ThresholdActions.Add(new ThresholdAction<T>(action));

        foreach (var action in timedActions)
            TimedActions.Add(new TimedAction<T>(action));

        foreach (var action in repeatedTimedActions)
            RepeatedTimedActions.Add(new TimedAction<T>(action));

        foreach (var action in timedActionSequences)
            TimedActionSequences.Add(new TimedActionSequence<T>(action));

        foreach (var action in repeatedTimedActionSequences)
            RepeatedTimedActionSequences.Add(new TimedActionSequence<T>(action));

        foreach (var action in conditionalActions)
            ConditionalActions.Add(new ConditionalAction<T>(action));

        foreach (var action in oneTimeConditionalActions)
            OneTimeConditionalActions.Add(new ConditionalAction<T>(action));
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        ScriptTimer.Update(delta);

        if (!ScriptTimer.IntervalElapsed)
            return;

        foreach (var action in ThresholdActions.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                ThresholdActions.Remove(action);

        foreach (var action in TimedActions.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                TimedActions.Remove(action);

        foreach (var action in RepeatedTimedActions)
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in TimedActionSequences.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                TimedActionSequences.Remove(action);

        foreach (var action in RepeatedTimedActionSequences.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in ConditionalActions)
            action.Update(Entity);

        foreach (var action in OneTimeConditionalActions.ToList())
            if (action.Update(Entity))
                OneTimeConditionalActions.Remove(action);
    }
}