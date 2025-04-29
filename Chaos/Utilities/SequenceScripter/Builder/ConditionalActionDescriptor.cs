#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public class ConditionalActionDescriptor<T> where T: Creature
{
    public Action<T> Action { get; }
    public Func<T, bool> Condition { get; }

    public ConditionalActionDescriptor(Func<T, bool> condition, Action<T> action)
    {
        Condition = condition;
        Action = action;
    }
}