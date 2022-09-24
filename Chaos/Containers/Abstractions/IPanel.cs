using Chaos.Common.Definitions;
using Chaos.Time.Abstractions;

namespace Chaos.Containers.Abstractions;

public interface IPanel<T> : IEnumerable<T>, IDeltaUpdatable
{
    T? this[byte slot] { get; }
    int AvailableSlots { get; }
    bool IsFull { get; }
    PanelType PaneType { get; }
    void AddObserver(Observers.Abstractions.IObserver<T> observer);
    bool Contains(T obj);
    bool IsValidSlot(byte slot);
    bool Remove(byte slot);
    bool Remove(string name);
    bool TryAdd(byte slot, T obj);
    bool TryAddToNextSlot(T obj);
    bool TryGetObject(byte slot, [MaybeNullWhen(false)] out T obj);
    bool TryGetRemove(byte slot, [MaybeNullWhen(false)] out T obj);
    bool TrySwap(byte slot1, byte slot2);
    void Update(byte slot, Action<T> action);
}