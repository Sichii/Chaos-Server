#region
using System.Runtime.InteropServices;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     A thread-safe collection of entities that can be linearly updated, allowing for the addition and removal of
///     entities during the update process
/// </summary>
/// <param name="logger">
///     A class logger used to log messages
/// </param>
/// <remarks>
///     If an entity is added or removed during the update process, the action is deferred until the update process is
///     complete. After the update, the deferred actions are processed in the order they were received
/// </remarks>
public sealed class UpdatableCollection(ILogger logger) : IDeltaUpdatable
{
    private readonly ILogger Logger = logger;

    private readonly List<IDeltaUpdatable> Objs = new();
    private readonly ConcurrentQueue<PendingAction> PendingActions = new();
    private bool IsUpdating;

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
                Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Update)
                      .LogError(e, "Error updating entity {@Entity}", obj);
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

    /// <summary>
    ///     Adds an object to the collection
    /// </summary>
    /// <param name="obj">
    ///     The object to add
    /// </param>
    public void Add(IDeltaUpdatable obj)
    {
        lock (this)
            if (!IsUpdating)
                Objs.Add(obj);
            else
                PendingActions.Enqueue(PendingAction.Add(obj));
    }

    /// <summary>
    ///     Clears the collection
    /// </summary>
    public void Clear()
    {
        lock (this)
            if (!IsUpdating)
                Objs.Clear();
            else
                PendingActions.Enqueue(PendingAction.Clear());
    }

    /// <summary>
    ///     Removes an object from the collection
    /// </summary>
    /// <param name="obj">
    ///     The object to remove
    /// </param>
    public void Remove(IDeltaUpdatable obj)
    {
        lock (this)
            if (!IsUpdating)
                Objs.Remove(obj);
            else
                PendingActions.Enqueue(PendingAction.Remove(obj));
    }

    internal sealed record PendingAction
    {
        internal IDeltaUpdatable? Obj { get; init; }
        internal required ActionType Type { get; init; }

        internal static PendingAction Add(IDeltaUpdatable obj)
            => new()
            {
                Obj = obj,
                Type = ActionType.Add
            };

        internal static PendingAction Clear()
            => new()
            {
                Type = ActionType.Clear
            };

        internal static PendingAction Remove(IDeltaUpdatable obj)
            => new()
            {
                Obj = obj,
                Type = ActionType.Remove
            };

        internal enum ActionType
        {
            Add,
            Remove,
            Clear
        }
    }
}