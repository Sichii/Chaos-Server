#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Collections;

public class AggroList : IEnumerable<KeyValuePair<uint, int>>
{
    private readonly ConcurrentDictionary<uint, int> Lookup = new();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<uint, int>> GetEnumerator() => Lookup.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int AddAggro(Creature creature, int value) => Lookup.AddOrUpdate(creature.Id, value, (_, oldValue) => oldValue + value);

    public void Clear(Creature? creature = null)
    {
        if (creature is null)
            Lookup.Clear();
        else
            Lookup.TryRemove(creature.Id, out _);
    }

    public int GetAggro(Creature creature) => Lookup.GetValueOrDefault(creature.Id);

    public int SetAggro(Creature creature, int value) => Lookup[creature.Id] = value;
}