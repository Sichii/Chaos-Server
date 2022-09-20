using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Core.JsonConverters;

namespace Chaos.Entities.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<LegendSchema, LegendMarkSchema>))]
public class LegendSchema : IEnumerable<LegendMarkSchema>
{
    public ICollection<LegendMarkSchema> LegendMarks { get; init; }

    public LegendSchema(IEnumerable<LegendMarkSchema> marks) => LegendMarks = marks.ToList();

    public IEnumerator<LegendMarkSchema> GetEnumerator() => LegendMarks.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}