#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Defines the base functionality for a panel
/// </summary>
/// <typeparam name="T">
///     The type of object that the panel will contain
/// </typeparam>
public abstract class PanelBase<T> : IPanel<T> where T: PanelEntityBase
{
    // <inheritdoc />
    public virtual int AvailableSlots
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return TotalSlots - Objects.Count(obj => obj != null);
        }
    }

    /// <summary>
    ///     The number of objects currently in the panel
    /// </summary>
    public virtual int Count
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return Objects.Count(obj => obj != null);
        }
    }

    /// <summary>
    ///     The slots that are considered invalid
    /// </summary>
    protected byte[] InvalidSlots { get; }

    // <inheritdoc />
    public virtual bool IsFull
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return Objects.Count(obj => obj != null) >= TotalSlots;
        }
    }

    /// <summary>
    ///     The length of the panel
    /// </summary>
    protected int Length { get; }

    /// <summary>
    ///     The objects contained within the panel
    /// </summary>
    protected T?[] Objects { get; }

    /// <summary>
    ///     The observers that are registered to this panel
    /// </summary>
    protected ICollection<Observers.Abstractions.IObserver<T>> Observers { get; }

    // <inheritdoc />
    public PanelType PanelType { get; }

    /// <summary>
    ///     The synchronization mechanism used to ensure thread safety
    /// </summary>
    protected Lock Sync { get; }

    /// <summary>
    ///     The total number of slots available in the panel
    /// </summary>
    protected int TotalSlots { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PanelBase{T}" /> class
    /// </summary>
    /// <param name="panelType">
    ///     The type of panel this is
    /// </param>
    /// <param name="length">
    ///     The length of the panel
    /// </param>
    /// <param name="invalidSlots">
    ///     The slots that are considered invalid
    /// </param>
    protected PanelBase(PanelType panelType, int length, byte[] invalidSlots)
    {
        PanelType = panelType;
        Length = length;
        Objects = new T[Length];
        InvalidSlots = invalidSlots;
        TotalSlots = Length - invalidSlots.Length;
        Sync = new Lock();
        Observers = new List<Observers.Abstractions.IObserver<T>>();
    }

    // <inheritdoc />
    public virtual void AddObserver(Observers.Abstractions.IObserver<T> observer)
    {
        using var @lock = Sync.EnterScope();
        Observers.Add(observer);
    }

    /// <inheritdoc />
    void IPanel<T>.Clear() => Array.Clear(Objects);

    // <inheritdoc />
    public virtual bool Contains(T obj)
    {
        using var @lock = Sync.EnterScope();

        return Objects.Any(o => (o != null) && o.Template.Name.EqualsI(obj.Template.Name));
    }

    // <inheritdoc />
    public virtual bool Contains(byte slot)
    {
        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.EnterScope();

        return Objects[slot] != null;
    }

    // <inheritdoc />
    public virtual bool Contains(string name)
    {
        using var @lock = Sync.EnterScope();

        return this.Any(obj => obj.Template.Name.EqualsI(name));
    }

    /// <inheritdoc />
    public bool ContainsByTemplateKey(string templateKey)
    {
        using var @lock = Sync.EnterScope();

        return this.Any(obj => obj.Template.TemplateKey.EqualsI(templateKey));
    }

    /// <inheritdoc />
    void IPanel<T>.ForceAdd(T obj) => Objects[obj.Slot] = obj;

    // <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        List<T?> snapshot;

        using (Sync.EnterScope())
            snapshot = Objects.ToList();

        return snapshot.Where((obj, index) => (obj != null) && IsValidSlot((byte)index))
                       .GetEnumerator()!;
    }

    // <inheritdoc />
    public virtual bool IsValidSlot(byte slot) => (slot > 0) && (slot < Length) && !InvalidSlots.Contains(slot);

    // <inheritdoc />
    public virtual T? this[byte slot]
    {
        get
        {
            using var @lock = Sync.EnterScope();

            if (IsValidSlot(slot))
                return Objects[slot];

            return default;
        }
    }

    // <inheritdoc />
    public virtual T? this[string name]
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));
        }
    }

    // <inheritdoc />
    public virtual bool Remove(string name)
    {
        using var @lock = Sync.EnterScope();

        var obj = this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));

        if (obj == null)
            return false;

        return Remove(obj.Slot);
    }

    // <inheritdoc />
    public virtual bool Remove(byte slot)
    {
        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.EnterScope();

        var existing = Objects[slot];

        if (existing == null)
            return false;

        Objects[slot] = null;
        BroadcastOnRemoved(slot, existing);

        return true;
    }

    /// <inheritdoc />
    public bool RemoveByTemplateKey(string templateKey)
    {
        using var @lock = Sync.EnterScope();

        var obj = this.FirstOrDefault(obj => obj.Template.TemplateKey.EqualsI(templateKey));

        if (obj == null)
            return false;

        return Remove(obj.Slot);
    }

    // <inheritdoc />
    public virtual bool TryAdd(byte slot, T obj) => InnerTryAdd(slot, obj);

    // <inheritdoc />
    public virtual bool TryAddToNextSlot(T obj)
    {
        using var @lock = Sync.EnterScope();

        for (byte i = 1; i < Length; i++)
            if ((Objects[i] == null) && IsValidSlot(i))
                return InnerTryAdd(i, obj);

        return false;
    }

    // <inheritdoc />
    public virtual bool TryGetObject(byte slot, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.EnterScope();

        obj = Objects[slot];

        return obj != null;
    }

    // <inheritdoc />
    public virtual bool TryGetObject(string name, [MaybeNullWhen(false)] out T obj)
    {
        using var @lock = Sync.EnterScope();

        var actualObjects = Objects.Where(obj => obj is not null)
                                   .ToList();

        obj = actualObjects.FirstOrDefault(obj => obj!.Template.Name.EqualsI(name));

        return obj != null;
    }

    // <inheritdoc />
    public virtual bool TryGetObjectByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj)
    {
        using var @lock = Sync.EnterScope();

        var actualObjects = Objects.Where(obj => obj is not null)
                                   .ToList();

        obj = actualObjects.FirstOrDefault(obj => obj!.Template.TemplateKey.EqualsI(templateKey));

        return obj != null;
    }

    // <inheritdoc />
    public virtual bool TryGetRemove(byte slot, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.EnterScope();

        obj = Objects[slot];

        if (obj == null)
            return false;

        Objects[slot] = default;
        BroadcastOnRemoved(slot, obj);

        return true;
    }

    /// <inheritdoc />
    public virtual bool TryGetRemove(string name, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        using var @lock = Sync.EnterScope();

        obj = this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));

        if (obj == null)
            return false;

        Objects[obj.Slot] = default;
        BroadcastOnRemoved(obj.Slot, obj);

        return true;
    }

    /// <inheritdoc />
    public bool TryGetRemoveByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        using var @lock = Sync.EnterScope();

        obj = this.FirstOrDefault(obj => obj.Template.TemplateKey.EqualsI(templateKey));

        if (obj == null)
            return false;

        Objects[obj.Slot] = default;
        BroadcastOnRemoved(obj.Slot, obj);

        return true;
    }

    // <inheritdoc />
    public virtual bool TrySwap(byte slot1, byte slot2)
    {
        if (!IsValidSlot(slot1) || !IsValidSlot(slot2))
            return false;

        using var @lock = Sync.EnterScope();

        var obj1 = Objects[slot1];
        var obj2 = Objects[slot2];

        if (obj1 != null)
        {
            BroadcastOnRemoved(slot1, obj1);
            obj1.Slot = slot2;
        }

        if (obj2 != null)
        {
            BroadcastOnRemoved(slot2, obj2);
            obj2.Slot = slot1;
        }

        Objects[slot1] = obj2;
        Objects[slot2] = obj1;

        if (obj1 != null)
            BroadcastOnAdded(obj1);

        if (obj2 != null)
            BroadcastOnAdded(obj2);

        return true;
    }

    // <inheritdoc />
    public void Update(TimeSpan delta)
    {
        using var @lock = Sync.EnterScope();

        foreach (var obj in Objects)
            obj?.Update(delta);
    }

    // <inheritdoc />
    public void Update(byte slot, Action<T>? action = null)
    {
        if (!IsValidSlot(slot))
            return;

        using var @lock = Sync.EnterScope();

        var obj = Objects[slot];

        if (obj == null)
            return;

        action?.Invoke(obj);
        BroadcastOnUpdated(slot, obj);
    }

    /// <summary>
    ///     Notifies all registered observers that a new object has been added.
    /// </summary>
    /// <param name="obj">
    ///     The object that was added
    /// </param>
    protected virtual void BroadcastOnAdded(T obj)
    {
        foreach (var observer in Observers)
            try
            {
                observer.OnAdded(obj);
            } catch
            {
                //ignored
            }
    }

    /// <summary>
    ///     Notifies all observers about the removal of an object from a specified slot
    /// </summary>
    /// <param name="slot">
    ///     The slot from which the object was removed
    /// </param>
    /// <param name="obj">
    ///     The object that was removed
    /// </param>
    protected virtual void BroadcastOnRemoved(byte slot, T obj)
    {
        foreach (var observer in Observers)
            try
            {
                observer.OnRemoved(slot, obj);
            } catch
            {
                //ignored
            }
    }

    /// <summary>
    ///     Notifies all observers about an update performed on the specified slot and its associated object
    /// </summary>
    /// <param name="originalSlot">
    ///     The original slot where the object resides
    /// </param>
    /// <param name="obj">
    ///     The object that has been updated
    /// </param>
    protected virtual void BroadcastOnUpdated(byte originalSlot, T obj)
    {
        foreach (var observer in Observers)
            try
            {
                observer.OnUpdated(originalSlot, obj);
            } catch
            {
                //ignored
            }
    }

    private bool InnerTryAdd(byte slot, T obj)
    {
        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.EnterScope();

        var existing = Objects[slot];

        if (existing != null)
            return false;

        Objects[slot] = obj;
        obj.Slot = slot;
        BroadcastOnAdded(obj);

        return true;
    }
}