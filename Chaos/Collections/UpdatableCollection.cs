using System.Runtime.InteropServices;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Collections;

public sealed class UpdatableCollection : IDeltaUpdatable
{
    private readonly ILogger Logger;

    private readonly List<IDeltaUpdatable> Objs;
    private readonly ConcurrentQueue<PendingAction> PendingActions;
    private bool IsUpdating;

    public UpdatableCollection(ILogger logger)
    {
        Logger = logger;
        Objs = new List<IDeltaUpdatable>();
        PendingActions = new ConcurrentQueue<PendingAction>();
    }

    public void Add(IDeltaUpdatable obj)
    {
        lock (this)
            if (!IsUpdating)
                Objs.Add(obj);
            else
                PendingActions.Enqueue(PendingAction.Add(obj));
    }

    public void Clear()
    {
        lock (this)
            if (!IsUpdating)
                Objs.Clear();
            else
                PendingActions.Enqueue(PendingAction.Clear());
    }

    public void Remove(IDeltaUpdatable obj)
    {
        lock (this)
            if (!IsUpdating)
                Objs.Remove(obj);
            else
                PendingActions.Enqueue(PendingAction.Remove(obj));
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        lock (this)
            IsUpdating = true;

        foreach (ref var obj in CollectionsMarshal.AsSpan(Objs))
            try
            {
                obj.Update(delta);
            } catch (Exception e)
            {
                Logger.LogError(e, "Error updating entity {Obj}", obj);
            }

        lock (this)
        {
            while (PendingActions.TryDequeue(out var action))
                switch (action.Type)
                {
                    case PendingAction.ActionType.Add:
                        Objs.Add(action.Obj!);

                        break;
                    case PendingAction.ActionType.Remove:
                        Objs.Remove(action.Obj!);

                        break;
                    case PendingAction.ActionType.Clear:
                        Objs.Clear();

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            IsUpdating = false;
        }
    }

    internal sealed record PendingAction
    {
        internal IDeltaUpdatable? Obj { get; init; }
        internal required ActionType Type { get; init; }

        internal static PendingAction Add(IDeltaUpdatable obj) => new() { Obj = obj, Type = ActionType.Add };

        internal static PendingAction Clear() => new() { Type = ActionType.Clear };
        internal static PendingAction Remove(IDeltaUpdatable obj) => new() { Obj = obj, Type = ActionType.Remove };

        internal enum ActionType
        {
            Add,
            Remove,
            Clear
        }
    }
}