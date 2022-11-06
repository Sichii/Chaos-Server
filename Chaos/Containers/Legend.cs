using Chaos.Objects.Legend;
using Chaos.Time;

namespace Chaos.Containers;

public sealed class Legend : IEnumerable<LegendMark>
{
    private readonly ConcurrentDictionary<string, LegendMark> Marks;

    public Legend(IEnumerable<LegendMark>? marks = null)
    {
        marks ??= Enumerable.Empty<LegendMark>();
        Marks = new ConcurrentDictionary<string, LegendMark>(marks.ToDictionary(mark => mark.Key), StringComparer.OrdinalIgnoreCase);
    }

    public bool AddOrAccumulate(LegendMark mark)
    {
        if (Marks.TryGetValue(mark.Key, out var existingMark))
        {
            existingMark.Count++;
            existingMark.Added = GameTime.Now;

            return true;
        }

        return Marks.TryAdd(mark.Key, mark);
    }

    public void AddUnique(LegendMark mark) => Marks[mark.Key] = mark;

    public bool Contains(LegendMark mark) => ContainsKey(mark.Key);

    public bool ContainsKey(string key) => Marks.ContainsKey(key);

    public int GetCount(string key)
    {
        if (Marks.TryGetValue(key, out var existingMark))
            return existingMark.Count;

        return 0;
    }

    public IEnumerator<LegendMark> GetEnumerator()
    {
        foreach (var kvp in Marks.OrderBy(kvp => kvp.Value.Added))
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Remove(string key, [MaybeNullWhen(false)] out LegendMark mark) => Marks.Remove(key, out mark);

    public bool RemoveCount(string key, int count, [MaybeNullWhen(false)] out LegendMark mark)
    {
        if (Marks.TryGetValue(key, out mark))
        {
            mark.Count -= count;

            if (mark.Count <= 0)
                return Marks.Remove(key, out mark);

            return true;
        }

        return false;
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out LegendMark existingMark) => Marks.TryGetValue(key, out existingMark);
}