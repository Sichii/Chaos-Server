using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Models.Panel.Abstractions;

namespace Chaos.Collections.Abstractions;

public abstract class PanelBase<T> : IPanel<T> where T: PanelEntityBase
{
    public virtual int AvailableSlots
    {
        get
        {
            using var @lock = Sync.Enter();

            return TotalSlots - Objects.Count(obj => obj != null);
        }
    }

    public virtual int Count
    {
        get
        {
            using var @lock = Sync.Enter();

            return Objects.Count(obj => obj != null);
        }
    }

    protected byte[] InvalidSlots { get; }

    public virtual bool IsFull
    {
        get
        {
            using var @lock = Sync.Enter();

            return Objects.Count(obj => obj != null) >= TotalSlots;
        }
    }

    protected int Length { get; }
    protected T?[] Objects { get; }

    protected ICollection<Observers.Abstractions.IObserver<T>> Observers { get; }

    public PanelType PanelType { get; }
    protected AutoReleasingMonitor Sync { get; }
    protected int TotalSlots { get; }

    protected PanelBase(PanelType panelType, int length, byte[] invalidSlots)
    {
        PanelType = panelType;
        Length = length;
        Objects = new T[Length];
        InvalidSlots = invalidSlots;
        TotalSlots = Length - invalidSlots.Length;
        Sync = new AutoReleasingMonitor();
        Observers = new List<Observers.Abstractions.IObserver<T>>();
    }

    public virtual void AddObserver(Observers.Abstractions.IObserver<T> observer)
    {
        using var @lock = Sync.Enter();
        Observers.Add(observer);
    }

    public virtual bool Contains(T obj)
    {
        using var @lock = Sync.Enter();

        return Objects.Any(o => (o != null) && o.Template.Name.EqualsI(obj.Template.Name));
    }

    public virtual bool Contains(byte slot)
    {
        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.Enter();

        return Objects[slot] != null;
    }

    public virtual bool Contains(string name)
    {
        using var @lock = Sync.Enter();

        return this.Any(obj => obj.Template.Name.EqualsI(name));
    }

    /// <inheritdoc />
    public bool ContainsByTemplateKey(string templateKey)
    {
        using var @lock = Sync.Enter();

        return this.Any(obj => obj.Template.TemplateKey.EqualsI(templateKey));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        List<T?> snapshot;

        using (Sync.Enter())
            snapshot = Objects.ToList();

        return snapshot.Where((obj, index) => (obj != null) && IsValidSlot((byte)index))
                       .GetEnumerator()!;
    }

    public virtual bool IsValidSlot(byte slot) => (slot > 0) && (slot < Length) && !InvalidSlots.Contains(slot);

    public virtual T? this[byte slot]
    {
        get
        {
            using var @lock = Sync.Enter();

            if (IsValidSlot(slot))
                return Objects[slot];

            return default;
        }
    }

    public virtual T? this[string name]
    {
        get
        {
            using var @lock = Sync.Enter();

            return this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));
        }
    }

    public virtual bool Remove(string name)
    {
        using var @lock = Sync.Enter();

        var obj = this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));

        if (obj == null)
            return false;

        return Remove(obj.Slot);
    }

    public virtual bool Remove(byte slot)
    {
        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.Enter();

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
        using var @lock = Sync.Enter();

        var obj = this.FirstOrDefault(obj => obj.Template.TemplateKey.EqualsI(templateKey));

        if (obj == null)
            return false;

        return Remove(obj.Slot);
    }

    public virtual bool TryAdd(byte slot, T obj) => InnerTryAdd(slot, obj);

    public virtual bool TryAddToNextSlot(T obj)
    {
        using var @lock = Sync.Enter();

        for (byte i = 1; i < Length; i++)
            if ((Objects[i] == null) && IsValidSlot(i))
                return InnerTryAdd(i, obj);

        return false;
    }

    public virtual bool TryGetObject(byte slot, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.Enter();

        obj = Objects[slot];

        return obj != null;
    }

    public virtual bool TryGetObject(string name, [MaybeNullWhen(false)] out T obj)
    {
        using var @lock = Sync.Enter();

        var actualObjects = Objects.Where(obj => obj is not null)
                                   .ToList();

        obj = actualObjects.FirstOrDefault(obj => obj!.Template.Name.EqualsI(name));

        return obj != null;
    }

    public virtual bool TryGetObjectByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj)
    {
        using var @lock = Sync.Enter();

        var actualObjects = Objects.Where(obj => obj is not null)
                                   .ToList();

        obj = actualObjects.FirstOrDefault(obj => obj!.Template.TemplateKey.EqualsI(templateKey));

        return obj != null;
    }

    public virtual bool TryGetRemove(byte slot, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        using var @lock = Sync.Enter();

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

        using var @lock = Sync.Enter();

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

        using var @lock = Sync.Enter();

        obj = this.FirstOrDefault(obj => obj.Template.TemplateKey.EqualsI(templateKey));

        if (obj == null)
            return false;

        Objects[obj.Slot] = default;
        BroadcastOnRemoved(obj.Slot, obj);

        return true;
    }

    public virtual bool TrySwap(byte slot1, byte slot2)
    {
        if (!IsValidSlot(slot1) || !IsValidSlot(slot2))
            return false;

        using var @lock = Sync.Enter();

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

    public void Update(TimeSpan delta)
    {
        using var @lock = Sync.Enter();

        foreach (var obj in Objects)
            obj?.Update(delta);
    }

    public void Update(byte slot, Action<T>? action = null)
    {
        if (!IsValidSlot(slot))
            return;

        using var @lock = Sync.Enter();

        var obj = Objects[slot];

        if (obj == null)
            return;

        action?.Invoke(obj);
        BroadcastOnUpdated(slot, obj);
    }

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

        using var @lock = Sync.Enter();

        var existing = Objects[slot];

        if (existing != null)
            return false;

        Objects[slot] = obj;
        obj.Slot = slot;
        BroadcastOnAdded(obj);

        return true;
    }
}