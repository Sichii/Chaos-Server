namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class TimedActionSequenceBuilder<T>
{
    private readonly List<TimedActionDescriptor<T>> Sequence = [];

    public TimedActionSequenceBuilder<T> AfterTime(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Sequence.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public TimedActionSequenceDescriptor<T> Build(TimeSpan startingAtTime)
        => new()
        {
            Sequence = Sequence,
            StartingAtTime = startingAtTime
        };

    public TimedActionSequenceDescriptor<T> Build()
        => new()
        {
            Sequence = Sequence
        };

    public TimedActionSequenceBuilder<T> ThenAfter(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Sequence.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }
}