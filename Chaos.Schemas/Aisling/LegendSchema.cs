using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<LegendSchema, LegendMarkSchema>))]
public sealed class LegendSchema : IEnumerable<LegendMarkSchema>
{
    public ICollection<LegendMarkSchema> LegendMarks { get; set; }

    public LegendSchema(IEnumerable<LegendMarkSchema> marks) => LegendMarks = marks.ToList();

    public IEnumerator<LegendMarkSchema> GetEnumerator() => LegendMarks.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}