using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record DialogTemplate : ITemplate
{
    public required string? NextDialogKey { get; init; }
    public required ICollection<DialogOption> Options { get; init; }
    public required string? PrevDialogKey { get; init; }
    public required ICollection<string> ScriptKeys { get; init; }
    public required IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public required string TemplateKey { get; init; }
    public required string Text { get; init; }
    public required ushort? TextBoxLength { get; init; }
    public required MenuOrDialogType Type { get; init; }

    public static DialogTemplate CloseDialogTemplate { get; } = new()
    {
        TemplateKey = "close",
        NextDialogKey = null,
        Options = Array.Empty<DialogOption>(),
        PrevDialogKey = null,
        ScriptKeys = Array.Empty<string>(),
        TextBoxLength = null,
        Text = string.Empty,
        Type = MenuOrDialogType.CloseDialog,
        ScriptVars = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase)
    };
}