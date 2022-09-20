using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Core.JsonConverters;

namespace Chaos.Entities.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<SkillBookSchema, SkillSchema>))]
public class SkillBookSchema : IEnumerable<SkillSchema>
{
    public ICollection<SkillSchema> Skills { get; init; }

    public SkillBookSchema(IEnumerable<SkillSchema> skills) => Skills = skills.ToList();

    public IEnumerator<SkillSchema> GetEnumerator() => Skills.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}