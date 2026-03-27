namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ConditionalActionDescriptor<T>
{
    public Action<T> Action { get; }
    public Func<T, bool> Condition { get; }

    public ConditionalActionDescriptor(Func<T, bool> condition, Action<T> action)
    {
        Condition = condition;
        Action = action;
    }
}