using Chaos.Schemas.Data;

namespace Chaos.Schemas.Content;

public class WorldMapSchema
{
    public required string WorldMapKey { get; init; }
    public required byte FieldIndex { get; init; }
    public required ICollection<string> NodeKeys { get; init; } = Array.Empty<string>();
}