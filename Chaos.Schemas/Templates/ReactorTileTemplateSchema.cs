using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Schemas.Templates;

public class ReactorTileTemplateSchema
{
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public bool ShouldBlockPathfinding { get; init; }
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
}