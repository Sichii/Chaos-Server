using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Serialization.Interfaces;

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

    public Bank(uint gold, IEnumerable<SerializableItem> serializableItems, ISerialTransformService<Item, SerializableItem> itemTransform)
    {
        Gold = gold;

        Items = serializableItems
                .Select(itemTransform.Transform)
                .ToDictionary(i => i.DisplayName, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        var enumerable = Enumerable.Empty<Item>();

        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}