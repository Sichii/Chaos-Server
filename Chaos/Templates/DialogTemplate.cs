using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record DialogTemplate : ITemplate
{
    public required bool Contextual { get; init; }
    public required string? NextDialogKey { get; init; }
    public required ICollection<DialogOption> Options { get; init; }
    public required string? PrevDialogKey { get; init; }
    public required ICollection<string> ScriptKeys { get; init; }
    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);
    public required string TemplateKey { get; init; }
    public required string Text { get; init; }
    public required ushort? TextBoxLength { get; init; }
    public required ChaosDialogType Type { get; init; }

    public static DialogTemplate CloseDialogTemplate { get; } = new()
    {
        TemplateKey = "close",
        NextDialogKey = null,
        Options = Array.Empty<DialogOption>(),
        PrevDialogKey = null,
        ScriptKeys = Array.Empty<string>(),
        TextBoxLength = null,
        Text = string.Empty,
        Type = ChaosDialogType.CloseDialog,
        ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
        Contextual = false
    };
}