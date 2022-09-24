namespace Chaos.Observers.Abstractions;

public interface IObserver<in T>
{
    void OnAdded(T obj);
    void OnRemoved(byte slot, T obj);
    void OnUpdated(byte originalSlot, T obj);
}