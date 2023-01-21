using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Schemas.Templates;

public class ReactorTileTemplateSchema
{
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public bool ShouldBlockPathfinding { get; set; }
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}