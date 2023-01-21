using Chaos.Common.Collections;

namespace Chaos.Schemas.Content;

public record ReactorTileSchema
{
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public bool ShouldBlockPathfinding { get; set; }
}