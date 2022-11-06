using Chaos.Common.Collections;
using Chaos.Scripting.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record MerchantTemplate : ITemplate, IScripted
{
    public required string? DialogKey { get; init; }
    public required string Name { get; init; }
    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public required ushort Sprite { get; init; }
    /// <inheritdoc />
    public required string TemplateKey { get; init; }
}