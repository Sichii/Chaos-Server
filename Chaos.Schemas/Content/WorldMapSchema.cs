using System.Text.Json.Serialization;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Content;

public class WorldMapSchema
{
    [JsonRequired]
    public string WorldMapKey { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte FieldIndex { get; init; }
    public ICollection<string> NodeKeys { get; init; } = Array.Empty<string>();
}