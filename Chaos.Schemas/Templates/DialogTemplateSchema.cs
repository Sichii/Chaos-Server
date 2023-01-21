using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

public sealed record DialogTemplateSchema
{
    public string? NextDialogKey { get; set; }
    public ICollection<DialogOptionSchema> Options { get; set; } = Array.Empty<DialogOptionSchema>();
    public string? PrevDialogKey { get; set; }
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
    [JsonRequired]
    public string Text { get; set; } = null!;
    public ushort? TextBoxLength { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MenuOrDialogType Type { get; set; }
}