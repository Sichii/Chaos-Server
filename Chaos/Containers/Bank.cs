using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;

namespace Chaos.Containers;

public class Bank : IEnumerable<Item>
{
    public uint Gold { get; set; }
    public Dictionary<string, Item> Items { get; set; }

    public Bank(IEnumerable<Item>? items = null)
    {
        items ??= Enumerable.Empty<Item>();

        Items = items.ToDictionary(item => item.DisplayName, StringComparer.OrdinalIgnoreCase);
    }

    public Bank(SerializableBank serializableBank, IItemFactory itemFactory)
    {
        Gold = serializableBank.Gold;

        foreach (var serializableItem in serializableBank.Items)
        {
            
        }
    }

    public IEnumerator<Item> GetEnumerator()
    {
        var enumerable = Enumerable.Empty<Item>();

        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}