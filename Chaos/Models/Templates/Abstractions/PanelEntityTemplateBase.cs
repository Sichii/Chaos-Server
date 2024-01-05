using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Models.Templates.Abstractions;

public abstract record PanelEntityTemplateBase : ITemplate, IScripted
{
    public required int AbilityLevel { get; init; }
    public required AdvClass? AdvClass { get; init; }
    public required BaseClass? Class { get; init; }
    public required TimeSpan? Cooldown { get; init; }
    public required string? Description { get; init; }
    public required int Level { get; init; }
    public required string Name { get; init; }
    public virtual required ushort PanelSprite { get; init; }
    public required bool RequiresMaster { get; init; }
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public required IDictionary<string, IScriptVars> ScriptVars { get; init; }
        = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);

    public required string TemplateKey { get; init; }
}