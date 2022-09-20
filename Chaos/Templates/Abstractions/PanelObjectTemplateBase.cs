using Chaos.Common.Definitions;
using Chaos.Core.Collections;
using Chaos.Data;
using Chaos.Scripts.Abstractions;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate, IScripted
{
    public required Animation? Animation { get; init; }
    public required BodyAnimation? BodyAnimationOverride { get; init; }
    public required TimeSpan? Cooldown { get; init; }
    public required string Name { get; init; }
    public virtual required ushort PanelSprite { get; init; }
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public required string TemplateKey { get; init; }
}