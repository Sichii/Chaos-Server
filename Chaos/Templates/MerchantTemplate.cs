using Chaos.Common.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record MerchantTemplate : ITemplate, IScripted
{
    public required string? DialogKey { get; init; }
    public required string Name { get; init; }
    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);
    public required ushort Sprite { get; init; }
    /// <inheritdoc />
    public required string TemplateKey { get; init; }
}