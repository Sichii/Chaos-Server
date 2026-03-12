namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class TimedActionDescriptor<T>
{
    public bool StartAsElapsed { get; init; }
    public Action<T> Action { get; }
    public TimeSpan Time { get; }

    public TimedActionDescriptor(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        Time = time;
        Action = action;
        StartAsElapsed = startAsElapsed;
    }
}