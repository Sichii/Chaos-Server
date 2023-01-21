using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

public class WorldMapSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte FieldIndex { get; set; }
    public ICollection<string> NodeKeys { get; set; } = Array.Empty<string>();
    [JsonRequired]
    public string WorldMapKey { get; set; } = null!;
}