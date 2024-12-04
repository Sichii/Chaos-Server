#region
using Chaos.DarkAges.Definitions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

public interface IPanel<T> : IEnumerable<T>, IDeltaUpdatable
{
    int AvailableSlots { get; }
    bool IsFull { get; }
    PanelType PanelType { get; }
    void AddObserver(Observers.Abstractions.IObserver<T> observer);
    void Clear();
    bool Contains(T obj);
    bool Contains(byte slot);
    bool Contains(string name);
    bool ContainsByTemplateKey(string templateKey);
    void ForceAdd(T obj);
    bool IsValidSlot(byte slot);
    T? this[byte slot] { get; }
    T? this[string name] { get; }
    bool Remove(byte slot);
    bool Remove(string name);
    bool RemoveByTemplateKey(string templateKey);
    bool TryAdd(byte slot, T obj);
    bool TryAddToNextSlot(T obj);
    bool TryGetObject(byte slot, [MaybeNullWhen(false)] out T obj);
    bool TryGetObject(string name, [MaybeNullWhen(false)] out T obj);
    bool TryGetObjectByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj);
    bool TryGetRemove(byte slot, [MaybeNullWhen(false)] out T obj);
    bool TryGetRemove(string name, [MaybeNullWhen(false)] out T obj);
    bool TryGetRemoveByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj);
    bool TrySwap(byte slot1, byte slot2);
    void Update(byte slot, Action<T>? action = null);
}