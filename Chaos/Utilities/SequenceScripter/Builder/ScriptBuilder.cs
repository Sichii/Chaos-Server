#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public class ScriptBuilder<T>
{
    protected readonly TimeSpan ScriptUpdateInterval;

    public ScriptBuilder(TimeSpan scriptUpdateInterval) => ScriptUpdateInterval = scriptUpdateInterval;

    public virtual ScriptedSequence<T> Build(T entity)
        => new(
            entity,
            ScriptUpdateInterval,
            RepeatedConditionalActions,
            RepeatedConditionalActionSequences,
            ConditionalActions,
            ConditionalActionSequences,
            RepeatedTimedActions,
            RepeatedTimedActionSequences,
            TimedActions,
            TimedActionSequences);

    #region Conditional
    protected readonly List<ConditionalActionDescriptor<T>> RepeatedConditionalActions = [];
    protected readonly List<ConditionalActionSequenceDescriptor<T>> RepeatedConditionalActionSequences = [];
    protected readonly List<ConditionalActionDescriptor<T>> ConditionalActions = [];
    protected readonly List<ConditionalActionSequenceDescriptor<T>> ConditionalActionSequences = [];
    #endregion

    #region Timed
    protected readonly List<TimedActionDescriptor<T>> RepeatedTimedActions = [];
    protected readonly List<TimedActionSequenceDescriptor<T>> RepeatedTimedActionSequences = [];
    protected readonly List<TimedActionDescriptor<T>> TimedActions = [];
    protected readonly List<TimedActionSequenceDescriptor<T>> TimedActionSequences = [];
    #endregion

    #region Conditional
    public virtual ScriptBuilder<T> WhileThenRepeatAction(Func<T, bool> condition, Action<T> action)
    {
        RepeatedConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public virtual ScriptBuilder<T> WhileThenRepeatSequence(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build();
        var descriptor = new ConditionalActionSequenceDescriptor<T>(condition, sequence);
        RepeatedConditionalActionSequences.Add(descriptor);

        return this;
    }

    public virtual ScriptBuilder<T> WhenThenDoActionOnce(Func<T, bool> condition, Action<T> action)
    {
        ConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public virtual ScriptBuilder<T> WhenThenDoSequenceOnce(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        ConditionalActionSequences.Add(new ConditionalActionSequenceDescriptor<T>(condition, builder.Build()));

        return this;
    }
    #endregion

    #region Timed
    public virtual ScriptBuilder<T> AfterTimeDoActionOnce(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        TimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public virtual ScriptBuilder<T> AfterTimeDoSequenceOnce(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    public virtual ScriptBuilder<T> AfterTimeRepeatAction(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public virtual ScriptBuilder<T> AfterTimeRepeatSequence(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }
    #endregion
}

public class CreatureScriptBuilder<T> : ScriptBuilder<T> where T: Creature
{
    protected readonly List<ThresholdActionDescriptor<T>> RepeatedThresholdActions = [];
    protected readonly List<ThresholdActionSequenceDescriptor<T>> RepeatedThresholdActionSequences = [];
    protected readonly List<ThresholdActionDescriptor<T>> ThresholdActions = [];
    protected readonly List<ThresholdActionSequenceDescriptor<T>> ThresholdActionSequences = [];

    public CreatureScriptBuilder(TimeSpan scriptUpdateInterval)
        : base(scriptUpdateInterval) { }

    public override ScriptedSequence<T> Build(T entity)
        => new CreatureScriptedSequence<T>(
            entity,
            ScriptUpdateInterval,
            RepeatedConditionalActions,
            RepeatedConditionalActionSequences,
            ConditionalActions,
            ConditionalActionSequences,
            RepeatedTimedActions,
            RepeatedTimedActionSequences,
            TimedActions,
            TimedActionSequences,
            RepeatedThresholdActions,
            RepeatedThresholdActionSequences,
            ThresholdActions,
            ThresholdActionSequences);

    #region Conditional
    public override CreatureScriptBuilder<T> WhileThenRepeatAction(Func<T, bool> condition, Action<T> action)
    {
        RepeatedConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public override CreatureScriptBuilder<T> WhileThenRepeatSequence(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build();
        var descriptor = new ConditionalActionSequenceDescriptor<T>(condition, sequence);
        RepeatedConditionalActionSequences.Add(descriptor);

        return this;
    }

    public override CreatureScriptBuilder<T> WhenThenDoActionOnce(Func<T, bool> condition, Action<T> action)
    {
        ConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public override CreatureScriptBuilder<T> WhenThenDoSequenceOnce(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        ConditionalActionSequences.Add(new ConditionalActionSequenceDescriptor<T>(condition, builder.Build()));

        return this;
    }
    #endregion

    #region Timed
    public override CreatureScriptBuilder<T> AfterTimeDoActionOnce(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        TimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public override CreatureScriptBuilder<T> AfterTimeDoSequenceOnce(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    public override CreatureScriptBuilder<T> AfterTimeRepeatAction(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public override CreatureScriptBuilder<T> AfterTimeRepeatSequence(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }
    #endregion

    #region Threshold Methods
    public CreatureScriptBuilder<T> AtThresholdDoActionOnce(int threshold, Action<T> action)
    {
        ThresholdActions.Add(new ThresholdActionDescriptor<T>(threshold, action));

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdDoSequenceOnce(int threshold, TimedActionSequenceBuilder<T> builder)
    {
        ThresholdActionSequences.Add(new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build()));

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdRepeatAction(int threshold, Action<T> action)
    {
        RepeatedThresholdActions.Add(new ThresholdActionDescriptor<T>(threshold, action));

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdRepeatSequence(int threshold, TimedActionSequenceBuilder<T> builder)
    {
        RepeatedThresholdActionSequences.Add(new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build()));

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdThenAfterTimeDoActionOnce(TimeSpan time, int threshold, Action<T> action)
    {
        ThresholdActions.Add(
            new ThresholdActionDescriptor<T>(threshold, action)
            {
                DelayAfterThreshold = time
            });

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdThenAfterTimeDoSequenceOnce(
        TimeSpan time,
        int threshold,
        TimedActionSequenceBuilder<T> builder)
    {
        var sequence = new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build())
        {
            DelayAfterThreshold = time
        };
        ThresholdActionSequences.Add(sequence);

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdThenAfterTimeRepeatAction(TimeSpan time, int threshold, Action<T> action)
    {
        RepeatedThresholdActions.Add(
            new ThresholdActionDescriptor<T>(threshold, action)
            {
                DelayAfterThreshold = time
            });

        return this;
    }

    public CreatureScriptBuilder<T> AtThresholdThenAfterTimeRepeatSequence(
        TimeSpan time,
        int threshold,
        TimedActionSequenceBuilder<T> builder)
    {
        var sequence = new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build())
        {
            DelayAfterThreshold = time
        };
        RepeatedThresholdActionSequences.Add(sequence);

        return this;
    }
    #endregion
}