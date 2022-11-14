using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<EffectsBarSchema, EffectSchema>))]
public class EffectsBarSchema : IEnumerable<EffectSchema>
{
    public ICollection<EffectSchema> Effects { get; init; }

    public EffectsBarSchema(IEnumerable<EffectSchema> marks) => Effects = marks.ToList();

    public IEnumerator<EffectSchema> GetEnumerator() => Effects.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}