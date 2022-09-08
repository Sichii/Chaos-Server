using Chaos.Data;
using Chaos.Scripts.Abstractions;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate, IScripted
{
    public Animation? Animation { get; init; }
    public TimeSpan? Cooldown { get; init; }
    public string Name { get; init; } = null!;
    public virtual ushort PanelSprite { get; init; }
    public ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public string TemplateKey { get; init; } = null!;
}