#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class TimedActionDescriptor<T> where T: Creature
{
    public bool StartAsElapsed { get; init; }
    public int? StartingAtHealthPercent { get; init; }
    public Action<T> Action { get; }
    public TimeSpan Time { get; }

    public TimedActionDescriptor(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Time = time;
        Action = action;
        StartAsElapsed = startAsElapsed;
    }
}