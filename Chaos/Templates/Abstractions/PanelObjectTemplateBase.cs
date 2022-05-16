using Chaos.Scripts.Interfaces;
using Chaos.Templates.Interfaces;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate, IScripted
{
    public Animation Animation { get; init; } = Animation.None;
    public TimeSpan BaseCooldown { get; init; } = TimeSpan.Zero;
    public string Name { get; init; } = "REPLACE ME";
    public virtual ushort PanelSprite { get; init; } = 0;
    public ICollection<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public abstract string TemplateKey { get; init; }
}