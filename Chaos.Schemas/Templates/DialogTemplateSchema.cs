using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

public sealed record DialogTemplateSchema
{
    public string? NextDialogKey { get; init; }
    public ICollection<DialogOptionSchema> Options { get; init; } = Array.Empty<DialogOptionSchema>();
    public string? PrevDialogKey { get; init; }
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
    [JsonRequired]
    public string Text { get; init; } = null!;
    public ushort? TextBoxLength { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MenuOrDialogType Type { get; init; }
}