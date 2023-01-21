using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

// ReSharper disable ArrangeAttributes

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<EffectsBarSchema, EffectSchema>))]
[JsonSerializable(typeof(ICollection<EffectSchema>))]
public class EffectsBarSchema : IEnumerable<EffectSchema>
{
    public ICollection<EffectSchema> Effects { get; set; }

    public EffectsBarSchema(IEnumerable<EffectSchema> marks) => Effects = marks.ToList();

    public IEnumerator<EffectSchema> GetEnumerator() => Effects.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}