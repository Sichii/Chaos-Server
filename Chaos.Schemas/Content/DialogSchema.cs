using Chaos.Common.Definitions;

namespace Chaos.Schemas.Content;

public sealed record DialogSchema
{
    public required string DialogKey { get; init; }
    public required string Text { get; init; }
    public required string? NextDialogKey { get; init; }
    public required string? PrevDialogKey { get; init; }
    public required MenuOrDialogType Type { get; init; }
    public required ICollection<DialogOptionSchema>? Options { get; init; }
    public required ICollection<string>? ItemTemplateKeys { get; init; }
    public required ICollection<string>? SpellTemplateKeys { get; init; }
    public required ICollection<string>? SkillTemplateKeys { get; init; }
    public required ICollection<string>? ScriptKeys { get; init; }
}   