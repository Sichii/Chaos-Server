namespace Chaos.Observers.Interfaces;

public interface IPanelObserver<in T>
{
    void OnAdded(T obj);
    void OnRemoved(byte slot, T obj);
    void OnUpdated(byte originalSlot, T obj);
}