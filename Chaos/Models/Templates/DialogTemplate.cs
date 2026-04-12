#region
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.Templates.Abstractions;
using Chaos.Schemas.Templates;
#endregion

namespace Chaos.Models.Templates;

public sealed record DialogTemplate : ITemplate
{
    public required bool Contextual { get; init; }

    /// <summary>
    ///     Per-dialog override for the client's NPC illustration variant index. See
    ///     <see cref="DialogTemplateSchema.IllustrationIndex" /> for details. Defaults to 0, which is correct for virtually
    ///     every NPC.
    /// </summary>
    public required byte IllustrationIndex { get; init; }

    public required string? NextDialogKey { get; init; }
    public required ICollection<DialogOption> Options { get; init; }
    public required string? PrevDialogKey { get; init; }
    public required ICollection<string> ScriptKeys { get; init; }

    public required IDictionary<string, IScriptVars> ScriptVars { get; init; }
        = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);

    public required string TemplateKey { get; init; }
    public required string Text { get; init; }
    public required ushort? TextBoxLength { get; init; }
    public required string? TextBoxPrompt { get; init; }
    public required ChaosDialogType Type { get; init; }
}