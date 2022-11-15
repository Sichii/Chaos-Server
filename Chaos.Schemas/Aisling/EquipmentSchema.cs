using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<EquipmentSchema, ItemSchema>))]
public sealed class EquipmentSchema : IEnumerable<ItemSchema>
{
    public ICollection<ItemSchema> Items { get; init; }

    public EquipmentSchema(IEnumerable<ItemSchema> items) => Items = items.ToList();

    public IEnumerator<ItemSchema> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}