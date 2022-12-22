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

    public bool ShouldBlockPathfinding { get; init; } = false;
    /// <inheritdoc />
    public required string TemplateKey { get; init; }
}