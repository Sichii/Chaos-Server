using Chaos.Common.Definitions;
using Chaos.Observers.Interfaces;
using Chaos.Time.Interfaces;

namespace Chaos.Containers.Interfaces;

public interface IPanel<T> : IEnumerable<T>, IDeltaUpdatable
{
    T? this[byte slot] { get; }
    int AvailableSlots { get; }
    bool IsFull { get; }
    PanelType PaneType { get; }
    void AddObserver(IPanelObserver<T> observer);
    bool Contains(T obj);
    bool IsValidSlot(byte slot);
    bool Remove(byte slot);
    bool Remove(string name);
    bool TryAdd(byte slot, T obj);
    bool TryAddToNextSlot(T obj);
    bool TryGetObject(byte slot, out T? obj);
    bool TryGetRemove(byte slot, out T? obj);
    bool TrySwap(byte slot1, byte slot2);
    void Update(byte slot, Action<T> action);
}