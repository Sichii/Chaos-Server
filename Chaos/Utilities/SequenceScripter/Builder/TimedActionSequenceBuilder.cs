#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public class TimedActionSequenceBuilder<T> where T: Creature
{
    private readonly List<TimedActionDescriptor<T>> Sequence = [];

    public TimedActionSequenceBuilder<T> AfterTime(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Sequence.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public TimedActionSequenceDescriptor<T> Build(int startingAtHealthPercent)
        => new()
        {
            Sequence = Sequence,
            StartingAtHealthPercent = startingAtHealthPercent
        };

    public TimedActionSequenceDescriptor<T> Build(TimeSpan startingAtTime)
        => new()
        {
            Sequence = Sequence,
            StartingAtTime = startingAtTime
        };

    public TimedActionSequenceDescriptor<T> Build(int startingAtHealthPercent, TimeSpan startingAtTime)
        => new()
        {
            Sequence = Sequence,
            StartingAtHealthPercent = startingAtHealthPercent,
            StartingAtTime = startingAtTime
        };

    public TimedActionSequenceBuilder<T> ThenAfter(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Sequence.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }
}