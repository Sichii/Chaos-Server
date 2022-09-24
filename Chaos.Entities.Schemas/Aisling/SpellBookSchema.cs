using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.JsonConverters;

namespace Chaos.Entities.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<SpellBookSchema, SpellSchema>))]
public class SpellBookSchema : IEnumerable<SpellSchema>
{
    public ICollection<SpellSchema> Spells { get; init; }

    public SpellBookSchema(IEnumerable<SpellSchema> spells) => Spells = spells.ToList();

    public IEnumerator<SpellSchema> GetEnumerator() => Spells.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}