#region
using Chaos.Models.World.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ConditionalAction<T> where T: Creature
{
    public Action<T> Action { get; }
    public Func<T, bool> Condition { get; }

    public ConditionalAction(ConditionalActionDescriptor<T> descriptor)
    {
        Condition = descriptor.Condition;
        Action = descriptor.Action;
    }

    public bool Update(T entity)
    {
        if (!Condition(entity))
            return false;

        Action(entity);

        return true;
    }
}