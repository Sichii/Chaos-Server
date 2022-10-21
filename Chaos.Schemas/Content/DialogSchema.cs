using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Content;

public sealed record DialogSchema
{
    [JsonRequired]
    public string DialogKey { get; init; } = null!;
    [JsonRequired]
    public string Text { get; init; } = null!;
    public string? NextDialogKey { get; init; }
    public string? PrevDialogKey { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MenuOrDialogType Type { get; init; }
    public ICollection<DialogOptionSchema> Options { get; init; } = Array.Empty<DialogOptionSchema>();
    public ICollection<string> ItemTemplateKeys { get; init; } = Array.Empty<string>();
    public ICollection<string> SpellTemplateKeys { get; init; } = Array.Empty<string>();
    public ICollection<string> SkillTemplateKeys { get; init; } = Array.Empty<string>();
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
}   