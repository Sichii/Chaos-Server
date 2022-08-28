using Chaos.Entities.Schemas.Data;

namespace Chaos.Entities.Schemas.Templates;

public record PanelObjectTemplateSchema
{
    public AnimationSchema? Animation { get; init; }
    public int CooldownMs { get; init; }
    public string Name { get; init; } = null!;
    public ushort PanelSprite { get; init; }
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public string TemplateKey { get; init; } = null!;
}