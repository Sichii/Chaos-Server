using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

public class WorldMapSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte FieldIndex { get; init; }
    public ICollection<string> NodeKeys { get; init; } = Array.Empty<string>();
    [JsonRequired]
    public string WorldMapKey { get; init; } = null!;
}