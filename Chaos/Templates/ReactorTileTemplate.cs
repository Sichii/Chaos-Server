using Chaos.Common.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed class ReactorTileTemplate : ITemplate, IScripted
{
    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);

    public required bool ShouldBlockPathfinding { get; init; }
    /// <inheritdoc />
    public required string TemplateKey { get; init; }
}