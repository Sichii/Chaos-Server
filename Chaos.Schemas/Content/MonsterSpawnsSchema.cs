using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Content;

[JsonConverter(typeof(EnumerableConverter<MonsterSpawnsSchema, MonsterSpawnSchema>))]
public class MonsterSpawnsSchema : IEnumerable<MonsterSpawnSchema>
{
    public ICollection<MonsterSpawnSchema> Values { get; init; }

    public MonsterSpawnsSchema(IEnumerable<MonsterSpawnSchema> values) => Values = values.ToList();

    /// <inheritdoc />
    public IEnumerator<MonsterSpawnSchema> GetEnumerator() => Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}