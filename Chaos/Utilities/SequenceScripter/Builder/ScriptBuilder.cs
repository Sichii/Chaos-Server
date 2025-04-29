#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ScriptBuilder<T> where T: Creature
{
    private readonly List<ConditionalActionDescriptor<T>> ConditionalActions = [];
    private readonly List<ConditionalActionDescriptor<T>> OneTimeConditionalActions = [];
    private readonly List<TimedActionDescriptor<T>> RepeatedTimedActions = [];
    private readonly List<TimedActionSequenceDescriptor<T>> RepeatedTimedActionSequences = [];
    private readonly TimeSpan ScriptUpdateInterval;
    private readonly List<ThresholdActionDescriptor<T>> ThresholdActions = [];
    private readonly List<TimedActionDescriptor<T>> TimedActions = [];
    private readonly List<TimedActionSequenceDescriptor<T>> TimedActionSequences = [];

    public ScriptBuilder(TimeSpan scriptUpdateInterval) => ScriptUpdateInterval = scriptUpdateInterval;

    public ScriptBuilder<T> AfterTime(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        TimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public ScriptBuilder<T> AfterTime(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptBuilder<T> AfterTimeStartingAtThreshold(
        TimeSpan time,
        int threshold,
        Action<T> action,
        bool startAsElapsed = false)
    {
        TimedActions.Add(
            new TimedActionDescriptor<T>(time, action, startAsElapsed)
            {
                StartingAtHealthPercent = threshold
            });

        return this;
    }

    public ScriptBuilder<T> AfterTimeStartingAtThreshold(TimeSpan time, int threshold, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(threshold, time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptBuilder<T> AtThreshold(int threshold, Action<T> action)
    {
        ThresholdActions.Add(new ThresholdActionDescriptor<T>(threshold, action));

        return this;
    }

    public ScriptedSequence<T> Build(T entity)
        => new(
            entity,
            ScriptUpdateInterval,
            ThresholdActions,
            TimedActions,
            RepeatedTimedActions,
            TimedActionSequences,
            RepeatedTimedActionSequences,
            ConditionalActions,
            OneTimeConditionalActions);

    public ScriptBuilder<T> RepeatEvery(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public ScriptBuilder<T> RepeatEvery(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptBuilder<T> RepeatEveryStartingAtThreshold(
        TimeSpan time,
        int threshold,
        Action<T> action,
        bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(
            new TimedActionDescriptor<T>(time, action, startAsElapsed)
            {
                StartingAtHealthPercent = threshold
            });

        return this;
    }

    public ScriptBuilder<T> RepeatEveryStartingAtThreshold(TimeSpan time, int threshold, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(threshold, time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptBuilder<T> WhenThen(Func<T, bool> condition, Action<T> action)
    {
        ConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public ScriptBuilder<T> WhenThenOnce(Func<T, bool> condition, Action<T> action)
    {
        OneTimeConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }
}