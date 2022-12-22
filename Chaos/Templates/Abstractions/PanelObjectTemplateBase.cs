using Chaos.Common.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate, IScripted
{
    public required TimeSpan? Cooldown { get; init; }
    public required string? Description { get; init; }
    public required string Name { get; init; }
    public virtual required ushort PanelSprite { get; init; }
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);
    public required string TemplateKey { get; init; }
}