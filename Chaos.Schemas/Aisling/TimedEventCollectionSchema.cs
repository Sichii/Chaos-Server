using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<TimedEventCollectionSchema, TimedEventSchema>))]
public sealed class TimedEventCollectionSchema : IEnumerable<TimedEventSchema>
{
    public ICollection<TimedEventSchema> Items { get; set; }

    public TimedEventCollectionSchema(IEnumerable<TimedEventSchema> items) => Items = items.ToList();

    public IEnumerator<TimedEventSchema> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}