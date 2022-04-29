using System.Collections;

namespace Chaos.Core.Collections.Synchronized;

public class SynchronizedList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly List<T> List;
    private readonly object Sync = new();

    public T this[int index]
    {
        get
        {
            lock (Sync)
                return List[index];
        }
        set
        {
            lock (Sync)
                List[index] = value;
        }
    }

    public int Count
    {
        get
        {
            lock (Sync)
                return List.Count;
        }
    }

    public bool IsReadOnly => false;

    public SynchronizedList(IEnumerable<T>? items = null)
    {
        items ??= Enumerable.Empty<T>();
        List = new List<T>(items);
    }

    public void Add(T item)
    {
        lock (Sync)
            List.Add(item);
    }

    public void Clear()
    {
        lock (Sync)
            List.Clear();
    }

    public bool Contains(T item)
    {
        lock (Sync)
            return List.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (Sync)
            List.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        lock (Sync)
            snapshot = List.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        lock (Sync)
            return List.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        lock (Sync)
            List.Insert(index, item);
    }

    public bool Remove(T item)
    {
        lock (Sync)
            return List.Remove(item);
    }

    public void RemoveAt(int index)
    {
        lock (Sync)
            List.RemoveAt(index);
    }
}