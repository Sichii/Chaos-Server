using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

public sealed record ReactorTileSchema
{
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public bool ShouldBlockPathfinding { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; init; }
}