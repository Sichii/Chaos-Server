using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chaos.Containers.Interfaces;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Observers.Interfaces;

namespace Chaos.Containers.Abstractions;

public abstract class PanelBase<T> : IPanel<T> where T: PanelObjectBase
{
    public T? this[byte slot]
    {
        get
        {
            lock (Sync)
                if (IsValidSlot(slot))
                    return Objects[slot];

            return default;
        }
    }

    public virtual int AvailableSlots
    {
        get
        {
            lock (Sync)
                return TotalSlots - Objects.Count(obj => obj != null);
        }
    }

    public virtual bool IsFull
    {
        get
        {
            lock (Sync)
                return Objects.Count(obj => obj != null) >= TotalSlots;
        }
    }

    public PanelType PaneType { get; }
    protected byte[] InvalidSlots { get; }
    protected int Length { get; }
    protected T?[] Objects { get; }

    protected ICollection<IPanelObserver<T>> Observers { get; }
    protected object Sync { get; }
    protected int TotalSlots { get; }

    protected PanelBase(
        PanelType panelType,
        int length,
        byte[] invalidSlots
    )
    {
        PaneType = panelType;
        Length = length;
        Objects = new T[Length];
        InvalidSlots = invalidSlots;
        TotalSlots = Length - invalidSlots.Length;
        Sync = new object();
        Observers = new List<IPanelObserver<T>>();
    }

    public void AddObserver(IPanelObserver<T> observer)
    {
        lock (Sync)
            Observers.Add(observer);
    }

    protected void BroadcastOnAdded(T obj)
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

    protected void BroadcastOnRemoved(byte slot, T obj)
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

    protected void BroadcastOnUpdated(byte originalSlot, T obj)
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

    public virtual bool Contains(T obj)
    {
        lock (Sync)
            return Objects.Contains(obj);
    }

    public IEnumerator<T> GetEnumerator()
    {
        List<T?> snapshot;

        lock (Sync)
            snapshot = Objects.ToList();

        using var enumerator = snapshot.GetEnumerator();
        byte index = 0;

        while (enumerator.MoveNext())
        {
            if ((enumerator.Current != null)
                && IsValidSlot(index))
                yield return enumerator.Current;

            index++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual bool IsValidSlot(byte slot) => (slot > 0) && (slot < Length) && !InvalidSlots.Contains(slot);

    public bool Remove(string name)
    {
        lock (Sync)
        {
            var obj = this.FirstOrDefault(obj => obj.Template.Name.EqualsI(name));

            if (obj == null)
                return false;

            return Remove(obj.Slot);
        }
    }

    public virtual bool Remove(byte slot)
    {
        if (!IsValidSlot(slot))
            return false;

        lock (Sync)
        {
            var existing = Objects[slot];

            if (existing == null)
                return false;

            Objects[slot] = null;
            BroadcastOnRemoved(slot, existing);

            return true;
        }
    }

    public virtual bool TryAdd(byte slot, T obj)
    {
        if (!IsValidSlot(slot))
            return false;

        lock (Sync)
        {
            var existing = Objects[slot];

            if (existing != null)
                return false;

            Objects[slot] = obj;
            obj.Slot = slot;
            BroadcastOnAdded(obj);

            return true;
        }
    }

    public virtual bool TryAddToNextSlot(T obj)
    {
        lock (Sync)
            for (byte i = 1; i < Length; i++)
                if ((Objects[i] == null) && IsValidSlot(i))
                    return TryAdd(i, obj);

        return false;
    }

    public virtual bool TryGetObject(byte slot, out T? obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        lock (Sync)
        {
            obj = Objects[slot];

            return true;
        }
    }

    public virtual bool TryGetRemove(byte slot, out T? obj)
    {
        obj = default;

        if (!IsValidSlot(slot))
            return false;

        lock (Sync)
        {
            obj = Objects[slot];

            if (obj == null)
                return false;

            Objects[slot] = default;
            BroadcastOnRemoved(slot, obj);

            return true;
        }
    }

    public virtual bool TrySwap(byte slot1, byte slot2)
    {
        if (!IsValidSlot(slot1) || !IsValidSlot(slot2))
            return false;

        lock (Sync)
        {
            var obj1 = Objects[slot1];
            var obj2 = Objects[slot2];

            if (obj1 != null)
                obj1.Slot = slot2;

            if (obj2 != null)
                obj2.Slot = slot1;

            Objects[slot1] = obj2;
            Objects[slot2] = obj1;

            if (obj1 != null)
                BroadcastOnUpdated(slot1, obj1);
            else if (obj2 != null)
                BroadcastOnRemoved(slot2, obj2);

            if (obj2 != null)
                BroadcastOnUpdated(slot2, obj2);
            else if (obj1 != null)
                BroadcastOnRemoved(slot1, obj1);

            return true;
        }
    }

    public void Update(byte slot, Action<T>? action = null)
    {
        if (!IsValidSlot(slot))
            return;

        lock (Sync)
        {
            var obj = Objects[slot];

            if (obj == null)
                return;

            action?.Invoke(obj);
            BroadcastOnUpdated(slot, obj);
        }
    }
}